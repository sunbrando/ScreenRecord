# ScreenRecord
 
点击此按钮就会把十秒内（可在代码进行修改）的Unity屏幕帧缓冲输出成gif  
![Image text](https://github.com/sunbrando/ScreenRecord/blob/master/Document/20200517221438.png)  
如下图的gif即由此插件生成的  
![inspector](https://github.com/sunbrando/ScreenRecord/blob/master/Assets/Gif/%E6%A1%88%E5%8F%91%E7%8E%B0%E5%9C%BA05-17%2018-19-19.gif)

用途：
---
1.可以做Unity的GIF屏幕录制工具  

2.当测试遇到bug的时候，一般都是发屏幕截图或者发报错堆栈给技术，  
但有些bug并不能简单的通过截图和堆栈就能明了，需要让测试重新把游戏跑一遍，然后你就傻傻的站在那看着他跑一遍，更严重的要重新创号跑一遍，  
更糟糕的是他说，我也不知道我刚刚怎么操作，就出这个bug了~  
这个时候，这个插件就派上用场了，开启时光倒流，案件瞬间重现，免去以前繁琐的操作，  
以后发现BUG不用再发图片，直接发GIF即可，测试看到这插件一定会感哭涕零~~~

已知BUG：
---
这里我是通过Camera.onPostRender获取的屏幕截图，fairyGUI/NGUI这些UI系统通过Camerah绘制的可以成功采样进去，而UGUI并不是通过Camera绘制，所以无法采样得到，暂时也还不知道怎么解决，有知道的同学希望能告知下。  

参考：
---
https://github.com/Chman/Moments  
之前一版是通过调用第三方GIF DLL形式实现的，后面朋友发现原来万能的GitHub已经有人做过类似（上面链接）的了，所以我也参考外国大佬的优化了自己的，并去掉了DLL直接使用源码的方式。  
我这个和外国大佬的最大的区别就是，他的代码比我好看...还有就是他获取帧缓冲的方式是OnRenderImage，而我用的是Camera.onPostRender，我测试了下OnRenderImage是无法采样到fairyGUI/NGUI这些UI系统的，不过同样无法采样到UGUI。

测试平台：
---
Unity2017.x/Unity2019.x  
支持编辑器和PC包的
