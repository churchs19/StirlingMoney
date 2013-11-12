using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class CategoryListViewModel : ObservableObject
	{
		private IRepository<Category, Guid> _categoryRepository;

		public CategoryListViewModel(IRepository<Category, Guid> categoryRepo)
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
			: this(KernelService.Kernel.Get<IRepository<Category, Guid>>())
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
			var navService = KernelService.Kernel.Get<INavigationService>();
			navService.Navigate<CategoryViewModel>();
		}

		public void LoadData()
		{
			Items.Clear();
			var categories = _categoryRepository.GetAllIndexKeys<string>("CategoryName");
			var categoryList = categories.OrderBy(it => it.Value).ToDictionary(key => key.Key, val => val.Value);
			foreach (var c in categories)
			{
				Items.Add(new CategoryViewModel() { CategoryId = c.Key, CategoryName = c.Value });
			}
		}
	}
}
