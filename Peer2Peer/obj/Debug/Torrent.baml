   M S B A M L   `   `   `  ����D  @Peer2Peer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Peer2Peer	Peer2Peer  5   	       Peer2Peer.Torrent   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:Peer2Peer     5   	   -   5          � Title$  Torrent��5   	   $	��450��6   $	��650��6&   .��5
      � ��5   
   �� 5       �� ImageSource$; 5C:\All Projects\Visual Studio\MiniTorrent\torrent.jpg��6    j� Stretch$ UniformToFill=�6]    � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      $��60 �6   �� 5      $	��230 �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $	��420 �6   �� 5      $��* �6   .+�5   
   �� $��Torrent��6   $��15(�6$   $
��Bold"�5   
   $��0��6   $��0��60   �� 5    
   -   5        $��70��5       $��Center=�6   $��Right=�69   $��SignOut��6W   $��0,0,10,0q�6i   $��1��6{   $��0��6�   6�   �� 5"   
   $��0��6   $��1��6'    �� Orientation$ 
Horizontal=�66   .+�5#      �� $��Request File��6   $
��Bold"�6-   $��Center=�6@   $��80��6]   �� 5$      -   5        $��requestFileTxtBox��5$      $�� ��6/   $��Center=�67   $	��100��5%      .��6   5&      �� 5'      -   5        $��70��5'      $��10,0,0,0q�6    $��Center=�63   $��Request��6P   6b   5(      �� 5*   
   $��0��6   $��2��6'   $ 
Horizontal=�66   .+�5+      �� -   5        $��listView��5+   #   $��10q�6    �� SelectionMode$ Single=�63    �� View 5,      �� 5-      .P�5.      �� $��	File name��6)    �� Width$	 100��6<   	 �� DisplayMemberBinding	 6H   �� *
FileName+�� 5/      $��	File size��6)   $	 100��6<   	 6H   �� *
FileSize+�� 50      $��Peers��6)   $	 100��68   	 6D   �� *NumberOfActiveUsers+51      53      55      �� 57   
   $��0��6   $��3��6'   $ 
Horizontal=�66   .+�58      � -   5        $��progressBar��58   2   $	��300��6   $��20��6&   $��Center=�6E   �� 59      $��Center=�6d   $
��Bold"�6�   $��Center=�6�   $��0,17,0,16.6q�6�   $��29��6�   ��6   �� 
 �� ElementName$
 progressBar�� �� Path$ Value� �� StringFormat$ {0:0}%��6�   �� 5:      -   5        $��70��5:      $��10,0,0,0q�6    $��Center=�63   $��Download��6P   6c   5<      �� 5>   
   $��1��6   $��3��6'   $ 
Horizontal=�66   .+�5?      �� -   5        $��labelDownload��5?      $�� ��6)   $
��Bold"�64   $��Center=�6F    J� RenderTransformOrigin$ 1.476,0.417&�6a   $��
0,10,0,0.2q�6�   $	��177��6�   $��89��6�   5@      �� 5B   
   $��1��6   $��2��6'   $ 
Horizontal=�66   .+�5C      �� -	   5         ��Name$ labelUpload��5C      $�� ��6)   $
��Bold"�64   $��Bottom=�6F   $	��177��6a   $��50��6m   5D      5E      5F      