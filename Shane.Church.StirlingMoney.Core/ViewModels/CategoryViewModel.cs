using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels.Shared;
using Shane.Church.StirlingMoney.Strings;
using Shane.Church.Utility.Core.Command;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class CategoryViewModel : ObservableObject, ICommittable
	{
		private IRepository<Category, Guid> _categoryRepository;
		private INavigationService _navService;

		public CategoryViewModel(IRepository<Category, Guid> categoryRepo, INavigationService navService)
		{
			if (categoryRepo == null) throw new ArgumentNullException("categoryRepo");
			_categoryRepository = categoryRepo;
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;

			EditCommand = new RelayCommand(NavigateToEdit);
			DeleteCommand = new AsyncRelayCommand(o => DeleteItem());
			SaveCommand = new AsyncRelayCommand(o => SaveItem());
		}

		public CategoryViewModel()
			: this(KernelService.Kernel.Get<IRepository<Category, Guid>>(), KernelService.Kernel.Get<INavigationService>())
		{
		}

		public CategoryViewModel(Category c)
			: this(KernelService.Kernel.Get<IRepository<Category, Guid>>(), KernelService.Kernel.Get<INavigationService>())
		{
			CategoryId = c.CategoryId;
			CategoryName = c.CategoryName;
		}

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

		public async Task LoadData(Guid categoryId)
		{
			if (categoryId != Guid.Empty)
			{
				var category = await _categoryRepository.GetEntryAsync(categoryId);
				if (category != null)
				{
					CategoryId = category.CategoryId;
					CategoryName = category.CategoryName;
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
			_navService.Navigate<CategoryViewModel>(this.CategoryId);
		}

		public ICommand DeleteCommand { get; private set; }

		public async Task DeleteItem()
		{
			await _categoryRepository.DeleteEntryAsync(CategoryId);
		}

		public ICommand SaveCommand { get; private set; }

		public delegate void ValidationFailedHandler(object sender, ValidationFailedEventArgs args);
		public event ValidationFailedHandler ValidationFailed;

		public async Task SaveItem()
		{
			var errors = Validate();
			if (errors.Count == 0)
			{
				Category c = new Category() { CategoryId = CategoryId, CategoryName = CategoryName };
				c = await _categoryRepository.AddOrUpdateEntryAsync(c);
				CategoryId = c.CategoryId;

				if (_navService.CanGoBack)
					_navService.GoBack();
			}
			else
			{
				if (ValidationFailed != null)
					ValidationFailed(this, new ValidationFailedEventArgs(errors));
			}
		}

		public async Task Commit()
		{
			await _categoryRepository.Commit();
		}
	}
}
