   M S B A M L   `   `   `  ����   7W    SevenWonders.TableUI   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Table Manager��$	��300��$	��511�� � WindowStartupLocation$ CenterScreen=�
�������.��� ����  �� ImageSource$3 -/7W;component/bin/Debug/Images/background.jpg�� � ColumnDefinitions �� $
��446* ��� $	��43* �.+��� -   $��border1��
�������$��1q�$	��196��$
��Left=�$	��Top=�$	��150��$��
321,18,0,0q�$��2���� -   $��label1��$��Your IP Address��$��28��$
��Left=�$��	12,12,0,0q�$	��Top=�$	��109��&   System.Windows.Controls.DataGrid  -   $��
playerList��$	��203��$
��Left=�$��	12,46,0,0q�$	��Top=�$	��214��   AutoGenerateColumns . �� -   $��addAIButton��$��Add AI��$��23��$
��Left=�$��
234,46,0,0q�$	��Top=�$��75���� -   $��removeAIButton��$��	Remove AI��$��23��$
��Left=�$��
234,80,0,0q�$	��Top=�$��75���� -   $��disbandButton��$��Disband��$��23��$
��Left=�$��234,197,0,0q�$	��Top=�$��75���� -   $��readyButton��$��Ready��$��23��$
��Left=�$��234,226,0,0q�$	��Top=�$��75��� -	   $��grid1��$	��243��$
��Left=�$��
319,18,0,0q�$	��Top=�$	��158��$��2��.+��� -
   $��scroll��$��0,0,0,35q����	#FFFFFFFC.���� -   $��chatTextBox��$
��Left=�$��6,6,0,0q�$�� ��$	��Top=�$
��Auto��$
��Wrap=� � DataContext �� �� -   $��chatTextField��$��29��$
��Left=�$��	6,208,0,0q�$	��Top=�$��91�����	#FFFFFFFC�� -   $��
sendButton��$
��Send��$��23��$��Right=�$��	0,214,6,0q�$	��Top=�$��49���� -   $��yourIPAddressField��$��23��$
��Left=�$��
106,14,0,0q�$	��Top=�$	��120�� �� 
IsReadOnly . {�.  �� -   $��leaders_Checkbox��$��Leaders��$��19��$
��Left=�$��234,109,0,0q�$	��Top=�$��75��