using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels.Shared;
using Shane.Church.StirlingMoney.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class CategoryViewModel : ObservableObject
	{
		public CategoryViewModel()
		{
			EditCommand = new RelayCommand(NavigateToEdit);
			DeleteCommand = new RelayCommand(DeleteItem);
			SaveCommand = new RelayCommand(SaveItem);
		}

		public CategoryViewModel(Category c)
		{
			_id = c.Id;
			_isDeleted = c.IsDeleted;
			CategoryId = c.CategoryId;
			CategoryName = c.CategoryName;
			EditCommand = new RelayCommand(NavigateToEdit);
			DeleteCommand = new RelayCommand(DeleteItem);
			SaveCommand = new RelayCommand(SaveItem);
		}

		private long? _id = null;
		private bool? _isDeleted = null;

		private Guid _categoryId;
		public Guid CategoryId
		{
			get { return _categoryId; }
			set
			{
				Set(() => CategoryId, ref _categoryId, value);
			}
		}

		private string _categoryName;
		public string CategoryName
		{
			get { return _categoryName; }
			set
			{
				Set(() => CategoryName, ref _categoryName, value);
			}
		}

		public void LoadData(Guid categoryId)
		{
			if (categoryId != Guid.Empty)
			{
				var categoryRepository = KernelService.Kernel.Get<IRepository<Category>>();
				var category = categoryRepository.GetFilteredEntries(it => it.CategoryId == categoryId).FirstOrDefault();
				if (category != null)
				{
					CategoryId = category.CategoryId;
					CategoryName = category.CategoryName;
					_id = category.Id;
					_isDeleted = category.IsDeleted;
				}
			}
		}

		public IList<string> Validate()
		{
			List<string> validationErrors = new List<string>();

			if (string.IsNullOrWhiteSpace(CategoryName))
			{
				validationErrors.Add(Resources.CategoryNameRequiredError);
			}

			return validationErrors;
		}

		public ICommand EditCommand { get; private set; }

		public void NavigateToEdit()
		{
			var navService = KernelService.Kernel.Get<INavigationService>();
			navService.Navigate<CategoryViewModel>(this.CategoryId);
		}

		public ICommand DeleteCommand { get; private set; }

		public void DeleteItem()
		{
			var categoryRepository = KernelService.Kernel.Get<IRepository<Category>>();
			categoryRepository.DeleteEntry(new Category() { CategoryId = CategoryId, Id = _id });
		}

		public ICommand SaveCommand { get; private set; }

		public delegate void ValidationFailedHandler(object sender, ValidationFailedEventArgs args);
		public event ValidationFailedHandler ValidationFailed;

		public void SaveItem()
		{
			var errors = Validate();
			if (errors.Count == 0)
			{
				var categoryRepository = KernelService.Kernel.Get<IRepository<Category>>();
				var navService = KernelService.Kernel.Get<INavigationService>();
				Category c = new Category() { CategoryId = CategoryId, CategoryName = CategoryName, Id = _id, IsDeleted = _isDeleted };
				c = categoryRepository.AddOrUpdateEntry(c);
				CategoryId = c.CategoryId;
				_id = c.Id;

				if (navService.CanGoBack)
					navService.GoBack();
			}
			else
			{
				if (ValidationFailed != null)
					ValidationFailed(this, new ValidationFailedEventArgs(errors));
			}
		}
	}
}
