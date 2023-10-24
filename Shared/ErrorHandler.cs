public class ErrorHandler
{
    public async Task<TryCatchResult<T>> TryCatchAsync<T>(Task<T> task, Action undoAction)	
    {
		bool succ = true;
		string msg = "ok";
		TryCatchResult<T> result;
		try
		{
			var res = await task;
			if (res.GetType() == typeof(HttpResponseMessage))
			{
				var httpMsg = res as HttpResponseMessage;
				succ = httpMsg.IsSuccessStatusCode;
				msg = httpMsg.ReasonPhrase ??= "unknown error";
			}
			result = new TryCatchResult<T>(msg, succ, res);
		}
		catch (Exception e)
		{
			result = new TryCatchResult<T>(e.Message, false, default);

			if (undoAction != null)
			{
				undoAction.Invoke();
			}
		}
		return result;
    }
}

public class TryCatchResult<T>
{
	public string Message { get; private set; }
	public bool Success { get; private set; }
	public T Result { get; private set; }

	public TryCatchResult()
	{

	}

	public TryCatchResult(string msg, bool success, T res)
	{
		Message= msg;
		Success= success;
		Result= res;
	}
}

