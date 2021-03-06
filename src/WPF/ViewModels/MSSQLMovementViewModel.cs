﻿using Phony.WPF.Data;
using System;
using System.Data.SqlClient;

namespace Phony.WPF.ViewModels
{
    public class MSSQLMovementViewModel : BaseViewModelWithAnnotationValidation
    {
        string _sqlServerName;
        string _sqlUserName;
        string _sqlPassword;
        string _sqlDataBase;
        bool _sqlUseDefault;
        bool _sqlIsWinAuth;
        bool _sqlIsSQLAuth;
        bool _sqlIsImporting;

        public string SQLServerName
        {
            get => _sqlServerName;
            set
            {
                _sqlServerName = value;
                NotifyOfPropertyChange(() => SQLServerName);
                NotifyOfPropertyChange(() => CanMoveData);
            }
        }

        public string SQLUserName
        {
            get => _sqlUserName;
            set
            {
                _sqlUserName = value;
                NotifyOfPropertyChange(() => SQLUserName);
                NotifyOfPropertyChange(() => CanMoveData);
            }
        }

        public string SQLPassword
        {
            get => _sqlPassword;
            set
            {
                _sqlPassword = value;
                NotifyOfPropertyChange(() => SQLPassword);
                NotifyOfPropertyChange(() => CanMoveData);
            }
        }

        public string SQLDataBase
        {
            get => _sqlDataBase;
            set
            {
                _sqlDataBase = value;
                NotifyOfPropertyChange(() => SQLDataBase);
                NotifyOfPropertyChange(() => CanMoveData);
            }
        }

        public bool SQLUseDefault
        {
            get => _sqlUseDefault;
            set
            {
                _sqlUseDefault = value;
                NotifyOfPropertyChange(() => SQLUseDefault);
                NotifyOfPropertyChange(() => CanMoveData);
            }
        }

        public bool SQLIsWinAuth
        {
            get => _sqlIsWinAuth;
            set
            {
                _sqlIsWinAuth = value;
                NotifyOfPropertyChange(() => SQLIsWinAuth);
                NotifyOfPropertyChange(() => CanMoveData);
            }
        }

        public bool SQLIsSQLAuth
        {
            get => _sqlIsSQLAuth;
            set
            {
                _sqlIsSQLAuth = value;
                NotifyOfPropertyChange(() => SQLIsSQLAuth);
                NotifyOfPropertyChange(() => CanMoveData);
            }
        }

        public bool SQLIsImporting
        {
            get => _sqlIsImporting;
            set
            {
                _sqlIsImporting = value;
                NotifyOfPropertyChange(() => SQLIsImporting);
                NotifyOfPropertyChange(() => CanMoveData);
            }
        }

        SqlConnectionStringBuilder SQLConnectionStringBuilder = new SqlConnectionStringBuilder();

        public MSSQLMovementViewModel()
        {
            SQLUseDefault = true;
            SQLIsWinAuth = true;
            SQLServerName = ".\\SQLExpress";
            SQLDataBase = "PhonyDb";
        }

        private bool CanMoveData
        {
            get
            {
                if (Properties.Settings.Default.IsConfigured)
                {
                    return false;
                }
                if (SQLUseDefault)
                {
                    return true;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(SQLServerName) && !string.IsNullOrWhiteSpace(SQLDataBase))
                    {
                        return !SQLIsSQLAuth || !string.IsNullOrWhiteSpace(SQLUserName) && !string.IsNullOrWhiteSpace(SQLPassword);
                    }
                }
                return false;
            }
        }

        public void MoveData()
        {
            SQLIsImporting = true;
            if (SQLUseDefault)
            {
                SQLConnectionStringBuilder.DataSource = ".\\SQLExpress";
                SQLConnectionStringBuilder.InitialCatalog = "PhonyDb";
                SQLConnectionStringBuilder.IntegratedSecurity = true;
                SQLConnectionStringBuilder.MultipleActiveResultSets = true;
            }
            else
            {
                SQLConnectionStringBuilder.DataSource = SQLServerName;
                SQLConnectionStringBuilder.InitialCatalog = SQLDataBase;
                if (SQLIsWinAuth)
                {
                    SQLConnectionStringBuilder.IntegratedSecurity = true;
                }
                else
                {
                    SQLConnectionStringBuilder.UserID = SQLUserName;
                    SQLConnectionStringBuilder.Password = SQLPassword;
                }
                SQLConnectionStringBuilder.MultipleActiveResultSets = true;
            }
            try
            {
                new SQL(SQLConnectionStringBuilder.ConnectionString).ImportFromMSSQL();

                Properties.Settings.Default.IsConfigured = true;
            }
            catch (Exception ex)
            {
                Properties.Settings.Default.IsConfigured = false;
                Core.SaveException(ex);
                MessageBox.MaterialMessageBox.ShowError("حدثت مشكلة اثناء عمليه النقل من فضلك تاكد من صحه البيانات", "خطأ", true);
            }
            finally
            {
                Properties.Settings.Default.Save();
                SQLIsImporting = false;
                if (Properties.Settings.Default.IsConfigured)
                {
                    MessageBox.MaterialMessageBox.Show("تم نقل بياناتك بنجاح", "تمت العملية", true);
                }
            }
        }


    }
}
