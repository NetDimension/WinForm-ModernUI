# ModernUI Theme for .Net WinForms

**ModernUI Theme** is a skin library that makes your .Net Windows Form Application look like ModernUI window in Win8/8.1 or Win10.

It redrews borders of stardard WinForm, and make a dropshadow around the window. You can set your form's border size and color, also you can set color of the dropshadow too.

![Screen Shot](http://ohtrip.cn/media/20180212015259.jpg)

## 2019/12/13 ##

.NET 4.6以及之后的版本微软为WinForm添加了原生的缩放支持，在app.config文件中加入一下代码即可开启原生的缩放支持。
```
<configuration>
    ...
    <System.Windows.Forms.ApplicationConfigurationSection>
        <add key="DpiAwareness" value="PerMonitorV2"/>
        <add key="EnableWindowsFormsHighDpiAutoResizing" value="true"/>
    </System.Windows.Forms.ApplicationConfigurationSection>
</configuration>
```
针对此特性，修改了处理DPI变化的消息，忽略原生的缩放下次，避免窗口被缩放2次的问题。

## 2019/11/11 更新 ##

加入了对Win8.1/Win10的PerMonitor/PerMonitorV2的DPI相关API的支持.

现在能过通过添加dpiAwareness属性来声明对不同DPI接口的支持。

```
    <application xmlns="urn:schemas-microsoft-com:asm.v3">
        <windowsSettings>
            <dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitorV2</dpiAwareness>
            <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true</dpiAware>
        </windowsSettings>
    </application>
```

## 2019/11/2 更新 ##

重新写了窗口底层解决了以下问题：

- 最大化最小化之后内容错乱
- 设计器大小与实际大小不一致
- 初始的 WindowState 设置为 Maximized 时窗口位置错误
- FormBorderStyle设置为None是全屏窗口大小不正确
- Win7+系统下，拖拽窗口到桌面顶部最大化，还原时窗口位置错误

修改的API

- 使用Borders属性替代了BorderWidth属性，现在可以使用Borders属性[Padding类型]来为窗体指定每条边框的大小
- 使用BorderColor属性替代了ActiveBorderColor和InactiveBorderColor属性，现在统一使用BorderColor属性设置边框颜色
- 使用ShadowColor属性替代了ActiveShaodowColor和InactiveShadowColor属性，现在统一使用ShadowColor属性设置窗体投影效果
- 移除了BorderEffect属性，现在只保留DropShadow一种投影模式，取消了Glow投影样式。

 

## Features
- Make ModernUI-like Forms for .Net Windows Form Applications.
- Full window animations support (Not just set FormBorderStyle to None).
- Fast draw the dropshadow around the form.
- Support Form Active/Inactive state.

## NuGet
```
PM> Install-Package NetDimension.WinForm.ModernUI
```


## Example


```C#
	public partial class Form1 : ModernUIForm
	{
		public Form1()
		{
			InitializeComponent();
		}
	}
```

Change properties in "UI" category to make your form style as your wish.

## Donate

If you like my work, please buy me a cup of coffee to encourage me continue with this library. 
![Screen Shot](http://ohtrip.cn/media/beg_with_border.png)

[![DONATE](http://ohtrip.cn/media/PayPal-donate-button.png)](https://www.paypal.me/mrjson)