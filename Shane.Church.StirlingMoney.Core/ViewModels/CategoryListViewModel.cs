using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
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
		}

		private ObservableCollection<CategoryViewModel> _items;
		public ObservableCollection<CategoryViewModel> Items
		{
			get { return _items; }
		}

		protected ICommand _addCommand;
		public ICommand AddCommand
		{
			get
			{
				if (_addCommand == null)
				{
					_addCommand = new RelayCommand(() =>
						{
							var navService = KernelService.Kernel.Get<INavigationService>();
							navService.Navigate<CategoryViewModel>();
						});
				}
				return _addCommand;
			}
		}

		public async Task LoadData()
		{
			var service = KernelService.Kernel.Get<IRepository<Category>>();
			var categories = await service.GetAllEntriesAsync();
			var categoryList = categories.OrderBy(it => it.CategoryName).ToList();
			foreach (var c in categories)
			{
				_items.Add(new CategoryViewModel(c));
			}
		}
	}
}
