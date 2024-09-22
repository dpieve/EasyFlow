﻿using System;
using System.Threading.Tasks;

namespace EasyFlow.Desktop.Services;
public interface INotificationService : IDisposable
{
    public Task Show(string title, string message);
    public Task Initialize();
}
