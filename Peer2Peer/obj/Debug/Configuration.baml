   M S B A M L   `   `   `  ����D  @Peer2Peer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Peer2Peer	Peer2Peer  5   	       Peer2Peer.Configuration   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:Peer2Peer     5   	     � Title$  MiniTorrent��5   	   $	��400��6   $	��525��6*   .��5
      � ��5   
   �� 5       �� ImageSource$? 9C:\All Projects\Visual Studio\MiniTorrent\torrentmain.jpg��6    j� Stretch$ UniformToFill=�6a    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $	��400 �6   �� 5      $��* �6   .+�5   
   �� $��Configuration��6   $��15(�6*   $
��Bold"�5   
   $��0��6   $��0��60   �� 5!   
   -   5        $��70��5!      $��Center=�6   $��SignIn��69   $��1��6J   $��0��6\   6k   �� 5#   
   $��0��6   $��1��6'    �� Orientation$ 
Horizontal=�66   .+�5$      �� $��Username��6   $
��Bold"�6)   $��Center=�6<   $��70��6Y   �� 5%      $��Center=�6A   $	��100��5&      ��5%      �� *
Username+ �� Mode$ TwoWay=�.��5&      5'      5(      �� 5*   
   $��0��6   $��2��6'   $ 
Horizontal=�66   .+�5+      �� $��Password��6   $
��Bold"�6)   $��Center=�6;   $��70��6X   G� 5,      -   5        $��txtPassword��5,      $��Center=�6/   $	��100��5-      5.      5/      �� 51   
   $��0��6   $��3��6'   $ 
Horizontal=�66   .+�52      �� $��IP��6   $
��Bold"�6#   $��Center=�65   $��70��6R   �� 53      $��Center=�6<   $	��100��54      ��53      �� *IP+$ TwoWay=�54      55      �� 57   
   $��0��6   $��4��6'   $ 
Horizontal=�66   .+�58      �� $
��PORT��6   $
��Bold"�6%   $��Center=�67   $��70��6T   �� 59      $��Center=�6=   $	��100��5:      ��59      �� *PORT+$ TwoWay=�5:      5;      �� 5=   
   $��0��6   $��5��6'   $ 
Horizontal=�66   .+�5>      �� $��
Files Path��6   $
��Bold"�6+   $��Center=�6=   $��70��6Z   �� 5?      -   5        $��70��5?      $��Center=�6    $��Folder��6=   6N   �� 5@      -   5        $��	LabelPath��5@   9   $��Path: ��6   $
��Bold"�6'   $��Center=�6J   $	��367��6g   5A      5C      5D      