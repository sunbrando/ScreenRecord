# ScreenRecord
 
点击此按钮就会把十秒内（可在代码进行修改）的Unity屏幕缓存输出成gif  
![Image text](https://github.com/sunbrando/ScreenRecord/blob/master/Document/20200517221438.png)  
如下图的gif即由此插件生成的  
![inspector](https://github.com/sunbrando/ScreenRecord/blob/master/Assets/Gif/%E6%A1%88%E5%8F%91%E7%8E%B0%E5%9C%BA05-17%2018-19-19.gif)

用途：
---
当测试遇到bug的时候，一般都是发屏幕截图或者发报错堆栈给技术，  
但有些bug并不能简单的通过截图和堆栈就能明了，需要让测试重新把游戏跑一遍，然后你就傻傻的站在那看着他跑一遍，更严重的要重新创号跑一遍，  
更糟糕的是他说，我也不知道我刚刚怎么操作，就出这个bug了~  
这个时候，这个插件就派上用场了，开启时光倒流，案件瞬间重现，免去以前繁琐的操作，  
以后发现BUG不用再发图片，直接发GIF即可，测试看到这插件一定会感哭涕零。

注意事项：
---
1.此插件只能用于编辑器和PC包</br>
2.需要将Api改为.NET 2.0，不能使用Subset，不然PC包无法使用System.Drawing</br>
![Image text](https://github.com/sunbrando/ScreenRecord/blob/master/Document/QQ%E5%9B%BE%E7%89%8720200517211712.png)  

已知BUG：
---
不能将UGUI的画面写入到gif，因为UGUI并不是通过Camera进行绘制，不过本人的项目用的是fairyGUI是通过Camerah绘制，暂时不知道怎么获取到UGUI的帧缓冲，有知道的同学希望能告知下。  
在自己宿舍的Unity2019.x打出的PC包发现System.Drawing会报错，即使将Api改为.NET 4.x~~~~  

测试平台：
---
Unity2017.x、Window
