   M S B A M L   `   `   `  ����G  CProxyClasses, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA0clr-namespace:ProxyClasses;assembly=ProxyClassesProxyClasses  5   	   ?/clr-namespace:ProxyServer;assembly=ProxyClassesProxyServer  5   	    ProxyServer#   ProxyServer.ProxyserverWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Clocal10clr-namespace:ProxyClasses;assembly=ProxyClasses     5   	   Clocal11/clr-namespace:ProxyServer;assembly=ProxyClasses     5   	     � Title$  ProxyServer��5	   	   $	��400��6   $	��800��6-   $	��450��6<   $	��800��6I    � 	Resources 5
         ProxyClasses.LogViewModel   logViewModel(   "ProxyServer.ProxySettingsViewModel  proxySettings��%:   &	        &	        5   
   6     5   
   6)   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��50��6   $
��377* �6-   �� 5      $��50��6   $
��415* �6-    � RowDefinitions 5   
   �� 5      $��50��6   $
��375* �6+   .+�5   
   � $��0��5      $��0��5      $��10,10,20,10q�5       5      �� 5      $��50��6#   $
��377* �61    5!      �� 5"      $	��10* �6    �� 5#      $��* �6    .+�5%      �� -   5         ��Name$ 
logListBox��5%   #   $��0��6   ��5&      �� 5'      .R�5(      �  5)      �� 5*   "   $
��auto �60   �� 5+   "   $
��auto �60    5-      �� 5.   "   .+�51      �� $��0��6$   $��0��61   ��6A   �� *Type+�� 52      $��1��6(   $��0��65   ��6E   �� *LogItemInfo+53      54      56      �� 57      -   5        $��1��57      .��67   	Clear Log6B   58      � 5:   
   -   5        $��settingsBlock��5;      $��0��5<      $��6,10,0,2q�5=      $��1��6    � DataContext# [ 5>       5A      �� 5B      $
��Auto �6    �� 5C      $
��Auto �6    �� 5D      $
��Auto �6    �� 5E      $
��Auto �6    �� 5F      $
��Auto �6    �� 5G      $
��Auto �6    �� 5H      $
��Auto �6    �� 5I      $
��Auto �6    �� 5J      $
��Auto �6    �� 5K      $
��Auto �6    �� 5L      $
��Auto �6    �� 5M      $
��Auto �6    �� 5N      $
��Auto �6     5P      �� 5Q      $
��Auto �6#   �� 5R      $��* �6#   .+�5U      �� $��1��6   $��0��6!   $��
Proxy port��61   $��Right=�6F   �� 5V      $��2��6   $��0��6!   $��Cache time out in seconds��61   $��Right=�6U   �� 5W      $��3��6   $��0��6!   $��Buffer size��61   $��Right=�6G   �� 5X      $��4��6   $��0��6!   $3��-Check op geweizigde content op server aan/uit��61   $��Right=�6i   �� 5Y      $��5��6   $��0��6!   $ ��Zet content filter aan/uit��61   $��Right=�6V   �� 5Z      $��6��6   $��0��6!   $��Verander headers aan/uit��61   $��Right=�6T   �� 5[      $��7��6   $��0��6!   $%��Set basis authenticatie aan/uit��61   $��Right=�6[   �� 5\      $��8��6   $��0��6!   $%��Logging request headers aan/uit��61   $��Right=�6\   �� 5]      $��9��6   $��0��6!   $��Logging content In��61   $��Right=�6O   �� 5^      $��10��6   $��0��6"   $��Logging content out��62   $��Right=�6Q   �� 5_      $��11��6   $��0��6"   $��Logging client info��62   $��Right=�6Q   �� 5a      -   5         ��Name$ txtPort��5a      $��1��6'   $��1��64   $��5q�6�   ��6D   �� *Port+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5b      -   5        $ txtCacheTimeout��5b      $��2��6/   $��1��6<   $��5q�6�   ��6M   �� *CacheTimeout+$ TwoWay=�$ PropertyChanged=�6�   �� 5c      -   5        $ txtBufferSize��5c      $��3��6-   $��1��6:   $	��5,4q�6�   ��6J   �� *
BufferSize+$ TwoWay=�$ PropertyChanged=�6�   �� 5d      -   5        	 ��Name$	 checkBoxContentFilter��5d      $��4��6�   $��1��6�   $��5.4,0,5.2,4.8q�6�   $��17��6�   $��Bottom=�6�   
 d� 	IsChecked
 66   �� *CheckModifiedContent+$ TwoWay=�$ PropertyChanged=�6�   �� 5e      -   5        $	 checkBoxChangeHeaders��5e      $��5��6�   $��1��6�   $��5.4,0,5.2,4.8q�6�   $��17��6�   $��Bottom=�6�   
 66   �� *ContentFilterOn+$ TwoWay=�$ PropertyChanged=�6�   �� 5f      -	   5        $	 checkBoxBasicAuth��5f      $��6��6�   $��1��6�   $��5.4,0,5.2,4.8q�6�   $��17��6�   $��Bottom=�6�   
 62   �� *AllowChangeHeaders+$ TwoWay=�$ PropertyChanged=�6�   �� 5g      -
   5        $"	 checkBoxCheckModifiedContent��5g   4   $��7��6   $��1��6$   $��5q�6�   
 6Z   �� *BasicAuthOn+$ TwoWay=�$ PropertyChanged=�6�   �� 5h      -   5        $	 checkBoxLogRequestHeaders��5h      $��8��6�   $��1��6�   $��5.4,0,5.2,4.8q�6�   $��17��6�   $��Bottom=�6�   
 6:   �� *LogRequestHeaders+$ TwoWay=�$ PropertyChanged=�6�   �� 5i      -   5        $	 checkBoxLogContentIN��5i      $��9��6�   $��1��6�   $��5.4,0,5.2,4.8q�6�   $��17��6�   $��Bottom=�6�   
 65   �� *LogContentIn+$ TwoWay=�$ PropertyChanged=�6�   �� 5j      -   5        $	 checkBoxLogContentOut��5j      $��10��6�   $��1��6�   $��5.4,0,5.2,4.8q�6�   $��17��6�   $��Bottom=�6�   
 66   �� *LogContentOut+$ TwoWay=�$ PropertyChanged=�6�   �� 5k      -   5        $	 checkBoxLogClient��5k      $��11��6�   $��1��6�   $��5.4,0,5.2,4.8q�6�   $��17��6�   $��Bottom=�6�   
 62   �� *LogCLientInfo+$ TwoWay=�$ PropertyChanged=�6�   �� 5m      -   5         ��Name$ btnStartStopProxy��5m      $��0��60   $��0��6=   $��2��6M   $��Start / Stop proxy��6a   $��5q�6~   $��10q�6�   6�   �� 5o      $��12��6   $��0��6"   $��Server Running:��62   �� 5p      -   5         ��Name$ button��5p      $��12��6$   $��1��62   $	��Top=�6�   ��6B   �� *ServerRunning+$ TwoWay=�$ PropertyChanged=�6�   5q      5r      5s      