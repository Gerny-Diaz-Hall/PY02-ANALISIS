using Avalonia.Controls;
using Avalonia.Interactivity;  // ¡Esta línea es la que faltaba!
using Avalonia.Threading;
using System;

namespace AvaloniaApplication1;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

}