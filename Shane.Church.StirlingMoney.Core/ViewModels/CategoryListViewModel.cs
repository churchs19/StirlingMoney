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
		public CategoryListViewModel()
		{
			_items = new ObservableCollection<CategoryViewModel>();
			_items.CollectionChanged += (s, e) =>
				{
					RaisePropertyChanged(() => Items);
				};
			AddCommand = new RelayCommand(AddCategory);
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

		public async Task LoadData()
		{
			var service = KernelService.Kernel.Get<IRepository<Category, Guid>>();
			Items.Clear();
			var categories = await service.GetAllEntriesAsync();
			var categoryList = categories.OrderBy(it => it.CategoryName).ToList();
			foreach (var c in categories)
			{
				Items.Add(new CategoryViewModel(c));
			}
		}
	}
}
