﻿using GalaSoft.MvvmLight;
using Grace;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.Utility.Core.Command;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class SettingsAppSyncUserViewModel : ObservableObject
	{
		private IDataRepository<AppSyncUser, string> _repository;

		public event ActionCompleteEventHandler SaveActionCompleted;
		public event ActionCompleteEventHandler RemoveActionCompleted;

		public SettingsAppSyncUserViewModel(IDataRepository<AppSyncUser, string> repository)
		{
			if (repository == null)
				throw new ArgumentNullException("repository");
			_repository = repository;

			_removeCommand = new AsyncRelayCommand(o => RemoveEntry());
			_saveCommand = new AsyncRelayCommand(o => SaveEntry());
		}

		private long? _id;
		public long? Id
		{
			get { return _id; }
			set
			{
				Set(() => Id, ref _id, value);
			}
		}

		private Guid _appSyncId;
		public Guid AppSyncId
		{
			get { return _appSyncId; }
			set
			{
				Set(() => AppSyncId, ref _appSyncId, value);
			}
		}

		private string _userEmail;
		public string UserEmail
		{
			get { return _userEmail; }
			set
			{
				Set(() => UserEmail, ref _userEmail, value);
			}
		}

		private bool _isRemovable;
		public bool IsRemovable
		{
			get { return _isRemovable; }
			protected set
			{
				Set(() => IsRemovable, ref _isRemovable, value);
			}
		}

		private ICommand _removeCommand;
		public ICommand RemoveCommand
		{
			get
			{
				return _removeCommand;
			}
		}

		private ICommand _saveCommand;
		public ICommand SaveCommand
		{
			get
			{
				return _saveCommand;
			}
		}

		public async Task LoadEntry(string userEmail)
		{
			var u = await _repository.GetEntryAsync(userEmail);
			LoadEntry(u);
		}

		public void LoadEntry(AppSyncUser user)
		{
			if (user != null)
			{
				AppSyncId = user.AppSyncId;
				UserEmail = user.UserEmail;
				IsRemovable = !user.IsSyncOwner;
			}
		}

		public async Task RemoveEntry()
		{
			await _repository.DeleteEntryAsync(UserEmail);
			if (RemoveActionCompleted != null)
				RemoveActionCompleted(this, new EventArgs());
		}

		public async Task SaveEntry()
		{
			var entry = ContainerService.Container.Locate<AppSyncUser>();
			entry.AppSyncId = AppSyncId;
			entry.UserEmail = UserEmail;
			await _repository.AddOrUpdateEntryAsync(entry);
			if (SaveActionCompleted != null)
				SaveActionCompleted(this, new EventArgs());
		}
	}
}
