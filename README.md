# ModernUI Theme for .Net WinForms

**ModernUI Theme** is a skin library that makes your .Net Windows Form Application look like ModernUI window in Win8/8.1 or Win10.

It redrews borders of stardard WinForm, and make a dropshadow around the window. You can set your form's border size and color, also you can set color of the dropshadow too.

![Screen Shot](http://ohtrip.cn/media/20180212015259.jpg)

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