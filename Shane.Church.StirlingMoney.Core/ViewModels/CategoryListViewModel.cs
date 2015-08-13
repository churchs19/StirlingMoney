using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Grace;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class CategoryListViewModel : ObservableObject
	{
		private IDataRepository<Category, Guid> _categoryRepository;

		public CategoryListViewModel(IDataRepository<Category, Guid> categoryRepo)
		{
			if (categoryRepo == null) throw new ArgumentNullException("categoryRepo");
			_categoryRepository = categoryRepo;
			_items = new ObservableCollection<CategoryViewModel>();
			_items.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => Items);
			};
			AddCommand = new RelayCommand(AddCategory);
		}

		public CategoryListViewModel()
			: this(ContainerService.Container.Locate<IDataRepository<Category, Guid>>())
		{
		}

		private ObservableCollection<CategoryViewModel> _items;
		public ObservableCollection<CategoryViewModel> Items
		{
			get { return _items; }
		}

		public ICommand AddCommand { get; protected set; }

		public void AddCategory()
		{
			var navService = ContainerService.Container.Locate<INavigationService>();
			navService.Navigate<CategoryViewModel>();
		}

		public void LoadData()
		{
			Items.Clear();
            var categories = _categoryRepository.GetAllEntries();
			foreach (var c in categories)
			{
				Items.Add(new CategoryViewModel() { CategoryId = c.CategoryId, CategoryName = c.CategoryName });
			}
		}
	}
}
