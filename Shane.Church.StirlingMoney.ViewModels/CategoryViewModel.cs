using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif
using System.Collections.Generic;
//using System.Data.Linq;

namespace Shane.Church.StirlingMoney.ViewModels
{
	public class CategoryViewModel : INotifyPropertyChanged
	{
		public CategoryViewModel()
		{
		}

		private Guid _categoryId;
		public Guid CategoryId
		{
			get { return _categoryId; }
			set
			{
				if (_categoryId != value)
				{
					_categoryId = value;
					NotifyPropertyChanged("CategoryId");
				}
			}
		}

		private string _categoryName;
		public string CategoryName
		{
			get { return _categoryName; }
			set
			{
				if (_categoryName != value)
				{
					_categoryName = value;
					NotifyPropertyChanged("CategoryName");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public void LoadData(Guid categoryId)
		{
			if (categoryId != Guid.Empty)
			{
#if !PERSONAL
				using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
				{
					var category = (from c in _context.Categories
									where c.CategoryId == categoryId
									select c);
#else
				var category = (from c in ContextInstance.Context.CategoryCollection
								where c.CategoryId == categoryId
								select c);
#endif
					if (category.Any())
					{
						Category c = category.FirstOrDefault();
						CategoryId = c.CategoryId;
						CategoryName = c.CategoryName;
					}
#if !PERSONAL
				}
#endif
			}
		}

		public IList<string> Validate()
		{
			List<string> validationErrors = new List<string>();

			if (string.IsNullOrWhiteSpace(CategoryName))
			{
				validationErrors.Add(Resources.ViewModelResources.CategoryNameRequiredError);
			}

			return validationErrors;
		}

		public void SaveCategory()
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				if (CategoryId != Guid.Empty)
				{
#if !PERSONAL
					var category = (from c in _context.Categories
									where c.CategoryId == CategoryId
									select c);
#else
				var category = (from c in ContextInstance.Context.CategoryCollection
								where c.CategoryId == CategoryId
								select c);
#endif
					if (category.Any())
					{
						Category c = category.First();
						c.CategoryName = CategoryName;
					}
				}
				else
				{
					Category c = new Category() { CategoryId = Guid.NewGuid(), CategoryName = CategoryName };
#if !PERSONAL
					_context.Categories.InsertOnSubmit(c);
#else
				ContextInstance.Context.AddCategory(c);
#endif
				}
#if !PERSONAL
				_context.SubmitChanges();
			}
#else
			ContextInstance.Context.SaveChanges();
#endif

		}
	}
}
