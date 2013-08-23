using GalaSoft.MvvmLight.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.Utility.Core.Command
{

	/// <summary>
	/// An async version for relay command
	/// </summary>
	public class AsyncRelayCommand<T> : ICommand
	{
		Func<bool> _canExecute;
		Func<object, Task<T>> _action;
		Func<T, Task> _completed;
		Action<Exception> _error;
		private bool _isExecuting = false;

		/// <summary>
		/// The constructor
		/// </summary>
		/// <param name="action">The action to be executed</param>
		/// <param name="canExecute">Will be used to determine if the action can be executed</param>
		/// <param name="completed">Will be invoked when the action is completed</param>
		/// <param name="error">Will be invoked if the action throws an error</param>
		public AsyncRelayCommand(Func<object,Task<T>> action,
									Func<bool> canExecute = null,
									Func<T, Task> completed = null,
									Action<Exception> error = null)
		{
			_action = action;
			_canExecute = canExecute;
			_completed = completed;
			_error = error;
		}

		/// <summary>
		/// Note that this will return false if the worker is already busy
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? !_isExecuting : (!_isExecuting) && _canExecute();
		}

		/// <summary>
		/// Let us use command manager for thread safety
		/// </summary>
		public event EventHandler CanExecuteChanged;

		private void ChangeIsExecuting(bool isExecuting)
		{
			if (isExecuting == _isExecuting) 
			{ return; 
			}
			_isExecuting = isExecuting;
			OnCanExecuteChanged();
		}

		protected void OnCanExecuteChanged()
		{
			var handler = CanExecuteChanged; 
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Here we'll invoke the background worker
		/// </summary>
		/// <param name="parameter"></param>
		public async void Execute(object parameter)
		{
			try
			{
				var result = await _action(parameter);
				if (_completed != null)
					await _completed(result);
			}
			catch (Exception ex)
			{
				if (_error != null)
					_error(ex);
			}
			finally
			{
				ChangeIsExecuting(false);
			}
		}
	}
}
