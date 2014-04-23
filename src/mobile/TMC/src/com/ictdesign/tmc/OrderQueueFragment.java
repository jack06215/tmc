/* COPYRIGHT (C) 2014 Carlo Chumroonridhi. All Rights Reserved. */

package com.ictdesign.tmc;

import java.util.ArrayList;

import Model.Order;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.media.MediaPlayer;
import android.os.Bundle;
import android.support.v4.app.ListFragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ListView;

/**
 * Sets the layout for the activity.
 * 
 * Creates a new adapter for a list of orders using predefined values.
 * 
 * Implements each order's "Delete" button and "Modify" option.
 */

public class OrderQueueFragment extends ListFragment
{
	MediaPlayer mMediaPlayer = new MediaPlayer();

	/**
	 * Implements the order's delete button.
	 */

	private OnClickListener mClickListener = new OnClickListener() {
		public void onClick(final View view)
		{
			new AlertDialog.Builder(getActivity())
					.setIcon(android.R.drawable.ic_delete)
					.setTitle(Constants.DELETE_TITLE)
					.setMessage(Constants.DELETE_CONFIRM)
					.setPositiveButton(Constants.OK,
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int id)
								{
									OrderQueueAdapter adapter = (OrderQueueAdapter) getListView()
											.getAdapter();
									// Remove the order obtained by
									// "view.getTag()" from the database
									// will be able to implement a
									// "Order.getId()" method, to pass to it.
									adapter.remove((Order) view.getTag());
									adapter.notifyDataSetChanged();
									playSound(R.raw.deleteorder);
								}
							})
					.setNegativeButton(Constants.CANCEL,
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int id)
								{
									dialog.cancel();
								}
							}).show();
		}
	};

	/**
	 * Implements the create order onClick Listener which starts up the new
	 * activity.
	 */

	private OnClickListener mCreateOrderListener = new OnClickListener() {
		public void onClick(View view)
		{
			Intent intent = new Intent(getActivity(), CreateOrderActivity.class);
			startActivityForResult(intent, Constants.REQUEST_CODE);
		}
	};

	/**
	 * Sets the layout for the activity.
	 * 
	 * Creates a new adapter for a list of orders using predefined values.
	 */
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState)
	{
		View rootView = inflater.inflate(R.layout.list_queue, container, false);
		((Button) rootView.findViewById(R.id.queuelist_createorder_b))
				.setOnClickListener(mCreateOrderListener);
		ArrayList<Order> orders = new ArrayList<Order>();
		// Get list of orders that are either active and pending
		for (Order order : Constants.ORDERS)
			if (!order.getOrderStatus().equals(Constants.COMPLETE))
				orders.add(order);
		// Replace list "orders" with the list of orders returned
		setListAdapter(new OrderQueueAdapter(getActivity(), R.layout.order_row,
				orders, mClickListener));
		return rootView;
	}

	/**
	 * Implements each order's onClick listener, displaying its current values
	 * and providing the option to modify the order.
	 */

	@Override
	public void onListItemClick(ListView l, View v, final int position, long id)
	{
		final Order order = (Order) getListAdapter().getItem(position);
		new AlertDialog.Builder(getActivity())
				.setIcon(
						((ImageView) v
								.findViewById(R.id.orderrow_orderstatus_iv))
								.getDrawable())
				.setTitle(order.getOrderName())
				.setTitle(order.getOrderName())
				.setMessage(
						String.format("%s: %s\n%s: %s\n%s: %s", Constants.NAME,
								order.getOrderName(), Constants.NUMBER,
								order.getOrderNumber(), Constants.STATUS,
								order.getOrderStatus()))
				.setPositiveButton(Constants.OK,
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog, int id)
							{
								dialog.cancel();
							}
						})
				.setNegativeButton(Constants.MODIFY,
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog, int id)
							{
								Intent intent = new Intent(getActivity(),
										ModifyOrderActivity.class);
								// Passes the order's ID and current values for
								// modification.
								intent.putExtra(Constants.ID, position);
								intent.putExtra(Constants.NAME,
										order.getOrderName());
								intent.putExtra(Constants.NUMBER,
										order.getOrderNumber());
								startActivityForResult(intent,
										Constants.REQUEST_CODE);
								dialog.cancel();
							}
						}).show();
	}

	/**
	 * Handles the return of the create and modify order activities.
	 * 
	 * Does some error-checking to ensure that necessary values have been
	 * passed.
	 */

	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data)
	{
		super.onActivityResult(requestCode, resultCode, data);
		switch (resultCode)
		{
		case Constants.CREATE_ORDER:
			if (data.hasExtra(Constants.NAME)
					&& data.hasExtra(Constants.NUMBER))
			{
				// Here is where we plug in the values of the new order.
				OrderQueueAdapter adapter = (OrderQueueAdapter) getListView()
						.getAdapter();
				adapter.add(new Order(data.getStringExtra(Constants.NAME), data
						.getStringExtra(Constants.NUMBER), Constants.PENDING));
				adapter.notifyDataSetChanged();
				// Up until here.
				playSound(R.raw.createorder);
			}
			break;

		case Constants.MODIFY_ORDER:
			if (data.hasExtra(Constants.ID))
				if (data.hasExtra(Constants.NAME))
					if (data.hasExtra(Constants.NUMBER))
					{
						// Here is where we modify the values of an existing
						// order.
						OrderQueueAdapter adapter = (OrderQueueAdapter) getListView()
								.getAdapter();
						Order order = adapter.getItem(data.getIntExtra(
								Constants.ID, 0));
						order.setOrderName(data.getStringExtra(Constants.NAME));
						order.setOrderNumber(data
								.getStringExtra(Constants.NUMBER));
						adapter.notifyDataSetChanged();
						// Up until here.
						playSound(R.raw.modifyorder);
					}
			break;
		}
	}

	/**
	 * Plays the sound of the id given.
	 * 
	 * @param soundId
	 */

	public void playSound(int songId)
	{
		if (mMediaPlayer.isPlaying())
			mMediaPlayer.stop();
		mMediaPlayer = MediaPlayer.create(getActivity(), songId);
		mMediaPlayer.setLooping(false);
		mMediaPlayer.start();
	}
}