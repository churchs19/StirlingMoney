using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Shane.Church.Utility.Core.WP
{
	public abstract class ChangingObservableObject : ObservableObject, INotifyPropertyChanging
	{
		#region INotifyPropertyChanging Members

		public event PropertyChangingEventHandler PropertyChanging;

		protected PropertyChangingEventHandler PropertyChangingHandler
		{
			get
			{
				return PropertyChanging;
			}
		}

		// Used to notify that a property is about to change
		protected virtual void RaisePropertyChanging(string propertyName)
		{
			if (PropertyChanging != null)
			{
				PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		protected virtual void RaisePropertyChanging<T>(Expression<Func<T>> propertyExpression)
		{
			if (PropertyChanging != null)
			{
				var propertyName = GetPropertyName(propertyExpression);
				PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Assigns a new value to the property. Then, raises the
		/// PropertyChanged event if needed. 
		/// </summary>
		/// <typeparam name="T">The type of the property that
		/// changed.</typeparam>
		/// <param name="propertyExpression">An expression identifying the property
		/// that changed.</param>
		/// <param name="field">The field storing the property's value.</param>
		/// <param name="newValue">The property's value after the change
		/// occurred.</param>
		/// <returns>True if the PropertyChanged event has been raised,
		/// false otherwise. The event is not raised if the old
		/// value is equal to the new value.</returns>
		new protected bool Set<T>(
			Expression<Func<T>> propertyExpression,
			ref T field,
			T newValue)
		{
			if (EqualityComparer<T>.Default.Equals(field, newValue))
			{
				return false;
			}

			RaisePropertyChanging(propertyExpression);
			field = newValue;
			RaisePropertyChanged(propertyExpression);
			return true;
		}

		/// <summary>
		/// Assigns a new value to the property. Then, raises the
		/// PropertyChanged event if needed. 
		/// </summary>
		/// <typeparam name="T">The type of the property that
		/// changed.</typeparam>
		/// <param name="propertyName">The name of the property that
		/// changed.</param>
		/// <param name="field">The field storing the property's value.</param>
		/// <param name="newValue">The property's value after the change
		/// occurred.</param>
		/// <returns>True if the PropertyChanged event has been raised,
		/// false otherwise. The event is not raised if the old
		/// value is equal to the new value.</returns>
		new protected bool Set<T>(
			string propertyName,
			ref T field,
			T newValue)
		{
			if (EqualityComparer<T>.Default.Equals(field, newValue))
			{
				return false;
			}

			RaisePropertyChanging(propertyName);
			field = newValue;
			RaisePropertyChanged(propertyName);
			return true;
		}
		#endregion
	}
}
