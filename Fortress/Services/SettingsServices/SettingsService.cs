using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace Fortress.Services.SettingsServices
{
    public class SettingsService
    {
        Template10.Services.SettingsService.ISettingsHelper _SettingsHelper;

        public SettingsService()
        {
            _SettingsHelper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public string Recent
        {
            get { return _SettingsHelper.Read(nameof(Recent), string.Empty); }
            set { _SettingsHelper.Write(nameof(Recent), value); }
        }


    }
}

