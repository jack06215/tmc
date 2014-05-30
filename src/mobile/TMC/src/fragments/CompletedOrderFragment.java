package fragments;


import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Locale;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import model.Constants;
import model.Order;
import ictd.activities.R;
import adapters.CompletedOrderAdapter;
import android.annotation.SuppressLint;
import android.app.AlertDialog;
import android.app.DatePickerDialog;
import android.app.Dialog;
import android.app.DialogFragment;
import android.app.ProgressDialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.support.v4.app.ListFragment;
import android.text.format.DateFormat;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.DatePicker;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

/**
 * Implements the CompletedOrderFragment, loads up the list fragment's layout,
 * plugs the list of completed orders into the adapter and set's the layout for
 * that as well. Implements the clicking of each order to display their
 * information.
 */

public class CompletedOrderFragment extends ListFragment
{
	private ResultReceiver receiver;
	private ProgressDialog pd;
	
	private OnClickListener mDateListener = new OnClickListener() {
		@Override
		public void onClick(View v)
		{
			DatePickerFragment newFragment = new DatePickerFragment();
			newFragment.setTextView((TextView) v);
			newFragment.show(getActivity().getFragmentManager(), "datePicker");		
		}
	};
	private TextView totalCount;

	/**
	 * Sets the layouts of the fragment and rows, places the list of completed
	 * orders into the respective adapter.
	 */

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState)
	{
		View rootView = inflater.inflate(R.layout.list_completed, container,
				false);

		setListAdapter(new CompletedOrderAdapter(getActivity(),
				R.layout.order_row, new ArrayList<Order>()));
		
		Calendar now = Calendar.getInstance();
		TextView from = (TextView) rootView
				.findViewById(R.id.completedorders_from_tv);
		TextView to = (TextView) rootView.findViewById(R.id.completedorders_to_tv);
		from.setText(DateFormat.format(Constants.DATE_FORMAT, now));
		from.setOnClickListener(mDateListener);
		to.setText(DateFormat.format(Constants.DATE_FORMAT, now));
		to.setOnClickListener(mDateListener);
		
    	totalCount = (TextView) rootView.findViewById(R.id.completedorders_totalcount_tv);
		
		return rootView;
	}

	/**
	 * Implements the clicking of each order. Displays their information in a
	 * dialog.
	 */

	@Override
	public void onListItemClick(ListView l, View v, int position, long id)
	{
		Order order = (Order) getListAdapter().getItem(position);
		new AlertDialog.Builder(getActivity())
				.setIcon(
						((ImageView) v
								.findViewById(R.id.orderrow_orderstatus_iv))
								.getDrawable())
				.setTitle(Integer.toString(order.getOrderId()))
				.setMessage(
						String.format(
								"%s: %s\n%s: %s\n%s: %s\n%s: %s\n\n%s: %d\n%s: %d\n%s: %d\n%s: %d\n%s: %d",
								Constants.NAME, order.getOrderOwner(),
								Constants.STATUS, order.getOrderStatus(),
								Constants.START_TIME, order.getStartTime(),
								Constants.FINISH_TIME, order.getFinishTime(),
								Constants.BLACK,
								order.getColourNumber(Constants.BLACK),
								Constants.BLUE,
								order.getColourNumber(Constants.BLUE),
								Constants.GREEN,
								order.getColourNumber(Constants.GREEN),
								Constants.RED,
								order.getColourNumber(Constants.RED),
								Constants.WHITE,
								order.getColourNumber(Constants.WHITE)))
				.setPositiveButton("Ok", new DialogInterface.OnClickListener() {
					public void onClick(DialogInterface dialog, int id)
					{
						dialog.cancel();
					}
				}).show();
	}

	
	
	
	
	// private class
	private class ResultReceiver extends BroadcastReceiver
	{
		@Override
		public void onReceive(Context context, Intent intent)
		{
			pd.dismiss();
			String response = intent.getStringExtra("result");
			handleCompletedOrders(response);
		}
	}

	
	
	
	
	private void handleCompletedOrders(String response)
	{
		Log.v("MAD", response);

		CompletedOrderAdapter adapter = (CompletedOrderAdapter) getListView() .getAdapter();
		adapter.clear();

		ArrayList<Order> orders = new ArrayList<Order>();
		JSONArray jArray;
		try
		{
			jArray = new JSONArray(response);

			for (int i = 0; i < jArray.length(); i++)
			{
				JSONObject jObj = jArray.getJSONObject(i);

				Order order = new Order(jObj.getInt("mOrderId"),
						jObj.getString("mOrderOwner"),
						jObj.getString("mOrderStatus"), jObj.getInt("black"),
						jObj.getInt("blue"), jObj.getInt("green"),
						jObj.getInt("red"), jObj.getInt("white"));
				String date = jObj.getString("endTime");
				order.setFinishTime(date);
				order.setStartTime(jObj.getString("startTime"));
				orders.add(order);
			}
		}
		catch (JSONException e)
		{
			Log.v("MAD", e.toString());
		}

		adapter.addAll(orders);
		adapter.notifyDataSetChanged();
        totalCount.setText(Integer.toString(orders.size()));
		
	}

	
	
	
	public void onStart()
	{
		receiver = new ResultReceiver();
		getActivity().registerReceiver(	receiver, new IntentFilter(Integer.toString(Constants.UPDATE_COMPLETED_ORDERS_COMMAND)));

		// update completed orders here
		makeService(Constants.UPDATE_COMPLETED_ORDERS_COMMAND, getCurrentDate(), getCurrentDate());
		super.onStart();
	}

	
	
	
	private String getCurrentDate() 
	{
		Calendar calendar = Calendar.getInstance();
	
		SimpleDateFormat format = new SimpleDateFormat(Constants.DATE_FORMAT, Locale.ENGLISH);
	    try
	    {
		  String dt = format.format(calendar.getTime());
          return dt;
	    }
	    catch(Exception exc)
	   {
		 return null;
	   }
	}

	
	
	@Override
	public void onStop()
	{
		getActivity().unregisterReceiver(receiver);
		super.onStop();
	}

	
	
	
	private void makeService(int command, String from, String to)
	{
		Intent service = new Intent(getActivity(), services.SynchService.class);
		Bundle parcel = new Bundle();
		parcel.putString("from", from);
		parcel.putString("to", to);
		parcel.putInt("command", command);
		service.putExtra("parcel", parcel);

		// stop any already running services associated with this activity
		// getActivity().stopService(service);
		pd = ProgressDialog.show(getActivity(), null, "Contacting server");
		getActivity().startService(service);
	}

	
	
	
	
	
	
	
	
	@SuppressLint("ValidFragment")
	public class DatePickerFragment extends DialogFragment implements
			DatePickerDialog.OnDateSetListener
	{
		private TextView mTextView = null;

		public void setTextView(TextView textView)
		{
			mTextView = textView;
		}
		
		

		@Override
		public Dialog onCreateDialog(Bundle savedInstanceState)
		{
			Calendar calendar = Calendar.getInstance();
			SimpleDateFormat format = new SimpleDateFormat(
					Constants.DATE_FORMAT, Locale.ENGLISH);
			try
			{
				calendar.setTime(format.parse(mTextView.getText().toString()));
			}
			catch (java.text.ParseException e)
			{
				// Basically means a date hasn't been selected yet so defaults
				// to current date
				e.printStackTrace();
			}

			int year = calendar.get(Calendar.YEAR);
			int month = calendar.get(Calendar.MONTH);
			int day = calendar.get(Calendar.DAY_OF_MONTH);

			DatePickerDialog datePickerDialog = new DatePickerDialog(
					getActivity(), this, year, month, day);

			switch (mTextView.getId())
			{
			case R.id.completedorders_from_tv:
				try
				{
					calendar.setTime(format.parse(((TextView) getActivity()
							.findViewById(R.id.completedorders_to_tv))
							.getText().toString()));
				}
				catch (java.text.ParseException e)
				{
				}
				datePickerDialog.getDatePicker().setMaxDate(
						Calendar.getInstance().getTimeInMillis());
				break;
			case R.id.completedorders_to_tv:

				try
				{
					calendar.setTime(format.parse(((TextView) getActivity()
							.findViewById(R.id.completedorders_from_tv))
							.getText().toString()));
					datePickerDialog.getDatePicker().setMinDate(
							calendar.getTimeInMillis());
				}
				catch (java.text.ParseException e)
				{
					// Ignore as limit hasn't been set
					e.printStackTrace();
				}
				datePickerDialog.getDatePicker().setMaxDate(
						Calendar.getInstance().getTimeInMillis());
				break;
			}

			return datePickerDialog;
		}

		
		
		public void onDateSet(DatePicker view, int year, int month, int day)
		{
			mTextView.setText(new StringBuilder().append(day).append("/")
					.append(month + 1).append("/").append(year));
			
			if(mTextView.getId() == R.id.completedorders_to_tv)
			{
				TextView to = (TextView)getActivity().findViewById(R.id.completedorders_to_tv);
				TextView from = (TextView)getActivity().findViewById(R.id.completedorders_from_tv);
				makeService(Constants.UPDATE_COMPLETED_ORDERS_COMMAND, from.getText().toString(), to.getText().toString() );
			}
			
		}
	}

}
