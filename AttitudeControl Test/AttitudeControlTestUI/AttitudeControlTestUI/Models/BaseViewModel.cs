﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AttitudeControlTestUI.Models
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        //MVVM THINGIES
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChangedByExplicitName(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //private IMessenger _messengerInstance;

        //protected IMessenger MessengerInstance
        //{
        //    get
        //    {
        //        return this._messengerInstance ?? Messenger.Default;
        //    }
        //    set
        //    {
        //        this._messengerInstance = value;
        //    }
        //}
    }
}
