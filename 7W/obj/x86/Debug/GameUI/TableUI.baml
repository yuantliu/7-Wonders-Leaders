   M S B A M L   `   `   `  ����  7W    SevenWonders.TableUI   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Table Manager��5   	   $	��300��6   $	��511��6,    � WindowStartupLocation$ CenterScreen=�68   
�������.��5      � ��5   
   �� 5       �� ImageSource$3 -/7W;component/bin/Debug/Images/background.jpg��6    � ColumnDefinitions 5	   
   �� 5
      $
��446* �6   �� 5      $	��43* �6   .+�5   
   �� -   5        $��border1��5   b   
�������$��1q�6&   $	��196��6:   $
��Left=�6G   $	��Top=�6q   $	��150��6�   $��
321,18,0,0q�6�   $��2��6�   �� 5   
   -   5        $��label1��5   d   $��Your IP Address��6   $��28��6*   $
��Left=�66   $��	12,12,0,0q�6Q   $	��Top=�6r   $	��109��6�   &   System.Windows.Controls.DataGrid  5   
   -   5        $��
playerList��5   N   $	��203��6   $
��Left=�6    $��	12,46,0,0q�6;   $	��Top=�6`   $	��214��6x      AutoGenerateColumns . 6�   �� 5   
   -   5        $��addAIButton��5   ]   $��Add AI��6   $��23��6"   $
��Left=�6.   $��
234,46,0,0q�6I   $	��Top=�6p   $��75��6�   6�   �� 5   
   -   5        $��removeAIButton��5   `   $��	Remove AI��6   $��23��6%   $
��Left=�61   $��
234,80,0,0q�6L   $	��Top=�6v   $��75��6�   6�   �� 5   
   -   5        $��disbandButton��5   _   $��Disband��6   $��23��6#   $
��Left=�6/   $��234,197,0,0q�6J   $	��Top=�6t   $��75��6�   �� 5   
   -   5        $��readyButton��5   ]   $��Ready��6   $��23��6!   $
��Left=�6-   $��234,226,0,0q�6H   $	��Top=�6p   $��75��6�   6�   � 5   
   -	   5        $��grid1��5   K   $	��243��6   $
��Left=�6   $��
319,18,0,0q�67   $	��Top=�6X   $	��158��6p   $��2��6|   .+�5      �� -
   5        $��scroll��5      $��0,0,0,35q�6*   ���	#FFFFFFFC.��5      �� -   5        $��chatTextBox��5   H   $
��Left=�6   $��6,6,0,0q�67   $�� ��6[   $	��Top=�6c   $
��Auto��6{   $
��Wrap=�6�    � DataContext 6�   �� 6�   5      �� 5      -   5        $��chatTextField��5   P   $��29��6   $
��Left=�6"   $��	6,208,0,0q�6=   $	��Top=�6e   $��91��6}   ���	#FFFFFFFC6�   �� 5      -   5        $��
sendButton��5   _   $
��Send��6   $��23��6$   $��Right=�60   $��	0,214,6,0q�6L   $	��Top=�6q   $��49��6�   6�   5      �� 5   
   -   5        $��yourIPAddressField��5   M   $��23��6   $
��Left=�6   $��
106,14,0,0q�69   $	��Top=�6g   $	��120��6    �� 
IsReadOnly . {�.  6�   �� 5   
   -   5        $��leaders_Checkbox��5   a   $��Leaders��6   $��19��6%   $
��Left=�61   $��234,109,0,0q�6L   $	��Top=�6y   $��75��6�   6�   5      5      