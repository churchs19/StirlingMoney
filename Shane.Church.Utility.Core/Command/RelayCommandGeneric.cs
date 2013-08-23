using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.Utility.Core.Command
{
	public class RelayCommandGeneric<T> : RelayCommand
	{
		private readonly WeakFunc<T, bool> _funcExecute;

		public RelayCommandGeneric(Func<T, bool> execute)
			: this(execute, null)
		{

		}

		public RelayCommandGeneric(Func<T, bool> execute, Func<bool> canExecute)
			: base(() => { }, canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");
			_funcExecute = new WeakFunc<T, bool>(execute);
		}

		public override void Execute(object parameter)
		{
			if (CanExecute(parameter)
				&& _funcExecute != null
				&& (_funcExecute.IsStatic || _funcExecute.IsAlive))
			{
				T param = (T)parameter;
				var result = _funcExecute.Execute(param);
			}
		}
	}
}
