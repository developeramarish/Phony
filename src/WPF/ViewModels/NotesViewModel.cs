﻿using LiteDB;
using Phony.WPF.Data;
using Phony.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Phony.WPF.ViewModels
{
    public class NotesViewModel : BaseViewModelWithAnnotationValidation
    {
        long _noId;
        string _name;
        string _phone;
        string _searchText;
        string _notes;
        string _childName;
        string _childNo;
        bool _byName;
        bool _fastResult;
        bool _openFastResult;
        bool _isAddNoFlyoutOpen;
        Note _dataGridSelectedNo;

        ObservableCollection<Note> _numbers;

        public long NoId
        {
            get => _noId;
            set
            {
                _noId = value;
                NotifyOfPropertyChange(() => NoId);
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                NotifyOfPropertyChange(() => Phone);
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                NotifyOfPropertyChange(() => SearchText);
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        public string ChildName
        {
            get => _childName;
            set
            {
                _childName = value;
                NotifyOfPropertyChange(() => ChildName);
            }
        }

        public string ChildNo
        {
            get => _childNo;
            set
            {
                _childNo = value;
                NotifyOfPropertyChange(() => ChildNo);
            }
        }

        public bool ByName
        {
            get => _byName;
            set
            {
                _byName = value;
                NotifyOfPropertyChange(() => ByName);
            }
        }

        public bool FastResult
        {
            get => _fastResult;
            set
            {
                _fastResult = value;
                NotifyOfPropertyChange(() => FastResult);
            }
        }

        public bool OpenFastResult
        {
            get => _openFastResult;
            set
            {
                _openFastResult = value;
                NotifyOfPropertyChange(() => OpenFastResult);
            }
        }

        public bool IsAddNoFlyoutOpen
        {
            get => _isAddNoFlyoutOpen;
            set
            {
                _isAddNoFlyoutOpen = value;
                NotifyOfPropertyChange(() => IsAddNoFlyoutOpen);
            }
        }

        public Note DataGridSelectedNo
        {
            get => _dataGridSelectedNo;
            set
            {
                _dataGridSelectedNo = value;
                NotifyOfPropertyChange(() => DataGridSelectedNo);
            }
        }

        public ObservableCollection<Note> Numbers
        {
            get => _numbers;
            set
            {
                _numbers = value;
                NotifyOfPropertyChange(() => Numbers);
            }
        }

        public ObservableCollection<User> Users { get; set; }

        public NotesViewModel()
        {
            Title = "ارقام";
            ByName = true;
            using var db = new LiteDatabase(Properties.Settings.Default.LiteDbConnectionString);
            Numbers = new ObservableCollection<Note>(db.GetCollection<Note>(DBCollections.Notes).FindAll());
            Users = new ObservableCollection<User>(db.GetCollection<User>(DBCollections.Users).FindAll());
        }

        private bool CanAddNo()
        {
            return string.IsNullOrWhiteSpace(Name) ? false : true;
        }

        private void DoAddNo()
        {
            using var db = new LiteDatabase(Properties.Settings.Default.LiteDbConnectionString);
            var n = new Note
            {
                Name = Name,
                Phone = Phone,
                Group = NoteGroup.Numbers,
                Notes = Notes,
                CreateDate = DateTime.Now,
                //Creator = Core.ReadUserSession(),
                EditDate = null,
                Editor = null
            };
            db.GetCollection<Note>(DBCollections.Notes).Insert(n);
            Numbers.Add(n);
            MessageBox.MaterialMessageBox.Show("تم اضافة الرقم بنجاح", "تمت العملية", true);
        }

        private bool CanEditNo()
        {
            return string.IsNullOrWhiteSpace(Name) || NoId == 0 || DataGridSelectedNo == null ? false : true;
        }

        private void DoEditNo()
        {
            using var db = new LiteDatabase(Properties.Settings.Default.LiteDbConnectionString);
            var n = db.GetCollection<Note>(DBCollections.Notes).FindById(DataGridSelectedNo.Id);
            n.Name = Name;
            n.Phone = Phone;
            n.Notes = Notes;
            //n.Editor = Core.ReadUserSession();
            n.EditDate = DateTime.Now;
            db.GetCollection<Note>(DBCollections.Notes).Update(n);
            Numbers[Numbers.IndexOf(DataGridSelectedNo)] = n;
            NoId = 0;
            DataGridSelectedNo = null;
            MessageBox.MaterialMessageBox.Show("تم تعديل الرقم بنجاح", "تمت العملية", true);
        }

        private bool CanDeleteNo()
        {
            return DataGridSelectedNo == null ? false : true;
        }

        private void DoDeleteNo()
        {
            var result = MessageBox.MaterialMessageBox.ShowWithCancel($"هل انت متاكد من حذف الرقم {DataGridSelectedNo.Name}", "حذف الرقم", true);
            if (result == MessageBoxResult.OK)
            {
                using (var db = new LiteDatabase(Properties.Settings.Default.LiteDbConnectionString))
                {
                    db.GetCollection<Note>(DBCollections.Notes).Delete(DataGridSelectedNo.Id);
                    Numbers.Remove(DataGridSelectedNo);
                }
                DataGridSelectedNo = null;
                MessageBox.MaterialMessageBox.Show("تم حذف الرقم بنجاح", "تمت العملية", true);
            }
        }

        private bool CanSearch()
        {
            return string.IsNullOrWhiteSpace(SearchText) ? false : true;
        }

        private void DoSearch()
        {
            try
            {
                using var db = new LiteDatabase(Properties.Settings.Default.LiteDbConnectionString);
                Numbers = new ObservableCollection<Note>(db.GetCollection<Note>(DBCollections.Notes).Find(i => i.Name.Contains(SearchText)));
                if (Numbers.Count > 0)
                {
                    if (FastResult)
                    {
                        ChildName = Numbers.FirstOrDefault().Name;
                        ChildNo = Numbers.FirstOrDefault().Phone;
                        OpenFastResult = true;
                    }
                }
                else
                {
                    MessageBox.MaterialMessageBox.ShowWarning("لم يتم العثور على شئ", "غير موجود", true);
                }
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                MessageBox.MaterialMessageBox.ShowError("لم يستطع ايجاد ما تبحث عنه تاكد من صحه البيانات المدخله", "خطأ", true);
            }
        }

        private bool CanReloadAllNos()
        {
            return true;
        }

        private void DoReloadAllNos()
        {
            using var db = new LiteDatabase(Properties.Settings.Default.LiteDbConnectionString);
            Numbers = new ObservableCollection<Note>(db.GetCollection<Note>(DBCollections.Notes).FindAll());
        }

        private bool CanFillUI()
        {
            return DataGridSelectedNo == null ? false : true;
        }

        private void DoFillUI()
        {
            NoId = DataGridSelectedNo.Id;
            Name = DataGridSelectedNo.Name;
            Phone = DataGridSelectedNo.Phone;
            Notes = DataGridSelectedNo.Notes;
            IsAddNoFlyoutOpen = true;
        }

        private bool CanOpenAddNoFlyout()
        {
            return true;
        }

        private void DoOpenAddNoFlyout()
        {
            IsAddNoFlyoutOpen = IsAddNoFlyoutOpen ? false : true;
        }
    }
}