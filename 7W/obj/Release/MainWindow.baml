   M S B A M L   `   `   `  ����   7W    SevenWonders.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  	7 Wonders��$	��789��$
��1055�� � WindowStartupLocation$ CenterScreen=� � Icon$. (/7W;component/bin/Debug/Images/stage.png��.��� -   $��mainGrid��$	��776��$
��1031��$y�Visible=� � ColumnDefinitions �� $
��792* ��� $
��239* �.+�o� -   $��menu1��$��23��$	��Top=�$��2��.@�m� $��G_ame��.@�m� -   $��CreateButton��$��C_reate Game��m� -   $��
JoinButton��$��J_oin Table��m� -   $��NickNameButton��$��C_hange Nickname��m� -   $��
QuitButton��$��Q_uit���� -   $��currentAgeLabel��$��Current Age:��$��28��$
��Left=�$��	12,29,0,0q�$	��Top=�$	��300��$y�Hidden=��� -	   $��
currentAge��$��X��$��25��$
��Left=�$��	95,29,0,0q�$	��Top=�$��37��$y�Hidden=��� -
   $��playerPanel��$	��181��$
��Left=�$��	12,63,0,0q�$	��Top=�$	��780���� -   $��	handPanel��$	��206��$��
13,445,0,0q�$	��Top=�$
��Left=�$	��791�� �� Orientation$ 
Horizontal=�$��2���� -   $��chatTextField��$��23��$��
24,704,0,0q�$	��Top=�$
��Left=�$	��146��$y�Hidden=�$��1���� -   $��
sendButton��$
��Send��$��23��$��Right=�$��
0,704,15,0q�$	��Top=�$��36��$y�Hidden=�$��1���� -   $��stackPanel1��$	��189��$
��Left=�$��188,250,0,0q�$	��Top=�$	��214��$y�Hidden=�.+��� -   $��playedCardsLabel��$��Played Cards��$��25��$	��198���� -   $��stackPanel2��$��20��$	��200��$ 
Horizontal=�.+��� -   $��image1��$��20��$
��Fill=�$��20��$-��'/7W;component/bin/Debug/Images/blue.png���� -   $��bluePlayedCards��$��20��$	��179���� -   $��stackPanel3��$��20��$ 
Horizontal=�$	��200��.+��� -   $��image2��$��20��$
��Fill=�$��20��$.��(/7W;component/bin/Debug/Images/brown.png���� -   $��brownPlayedCards��$��20��$	��179���� -   $��stackPanel4��$��20��$ 
Horizontal=�$	��200��.+��� -   $��image3��$��20��$
��Fill=�$��20��$.��(/7W;component/bin/Debug/Images/green.png���� -   $��greenPlayedCards��$��20��$	��178���� -   $��stackPanel5��$��20��$ 
Horizontal=�$	��200��.+��� -   $��image4��$��20��$
��Fill=�$��20��$-��'/7W;component/bin/Debug/Images/grey.png���� -   $��greyPlayedCards��$��20��$	��178���� -   $��stackPanel6��$��20��$ 
Horizontal=�$	��200��.+��� -   $��image5��$��20��$
��Fill=�$��20��$/��)/7W;component/bin/Debug/Images/purple.png���� -   $��purplePlayedCards��$��20��$	��178���� -   $��stackPanel7��$��20��$ 
Horizontal=�$	��200��.+��� -    $��image6��$��20��$
��Fill=�$��20��$,��&/7W;component/bin/Debug/Images/red.png���� -!   $��redPlayedCards��$��20��$	��178���� -"   $��stackPanel8��$��20��$ 
Horizontal=�$	��200��.+��� -#   $��image7��$��20��$
��Fill=�$��20��$/��)/7W;component/bin/Debug/Images/yellow.png���� -$   $��yellowPlayedCards��$��20��$	��177���� -%   $��stackPanel9��$��20��$ 
Horizontal=�$	��200��.+��� -&   $��image9��$��20��$
��Fill=�$��20��$/��)/7W;component/bin/Debug/Images/leader.png���� -'   $��whitePlayedCards��$��20��$	��177���� -(   $��	cardImage��$	��199��$
��Left=�$��
13,250,0,0q�$
��Fill=�$	��Top=�$	��126���� -)   $��
boardImage��$	��162��$
��Left=�$��408,250,0,0q�$
��Fill=�$	��Top=�$	��395��$��2���� -*   $��scroll��$	��668��$
��Left=�$��	18,29,0,0q�$	��Top=�$	��206��$y�Hidden=�$��1��.���� -+   $��chatTextBox��$
��Left=�$��6,6,0,0q�$�� ��$	��Top=�$
��Auto��$
��Auto��$
��Wrap=� � DataContext �� �� -,   $��actionBuildPanel��$��25��$
��Left=�$��
12,657,0,0q�$	��Top=�$	��791��$ 
Horizontal=�$��2���� --   $��actionStagePanel��$��25��$
��Left=�$��
12,688,0,0q�$	��Top=�$	��791��$ 
Horizontal=�$��2���� -.   $��actionDiscardPanel��$��25��$
��Left=�$��
12,719,0,0q�$	��Top=�$	��791��$ 
Horizontal=�$��2���� -/   $��timerTextBox��$��25��$
��Left=�$��742,219,0,0q�$	��Top=�$��62��$y�Hidden=�$��2���� -0   $��currentStageLabel��$��27��$
��Left=�$��408,414,0,0q�$	��Top=�$	��125���� -1   $��olympiaButton��$��23��$
��Left=�$��539,414,0,0q�$	��Top=�$��23��$y�Hidden=�{�.  ����  �� ImageSource$; 5/7W;component/bin/Debug/Images/olympiapowerbutton.png���� -2   $��bilkisButton��$��568,414,201,339q�$y�Hidden=�{�.  ���� $: 4/7W;component/bin/Debug/Images/bilkispowerbutton.png���� -3   $��estebanButton��$��23��$��597,414,172,339q�$y�Hidden=�{�.  ���� $; 5/7W;component/bin/Debug/Images/estebanpowerbutton.png���� -4   $��canvas1��$	��351��$
��Left=�$��487,300,0,0q�$	��Top=�$	��256��.+��� -5   $��joinGameIcon�� �� Left$ 22��
 �� Top$	 126��$��74��$
��Fill=�$	��184��$1��+/7W;component/bin/Debug/Images/joinGame.png���� -6   $��image8��$��78��$
��Fill=�$	��205��$0��*/7W;component/bin/Debug/Images/newGame.png��$ 6��$ 42���� -7   $��quitIcon��$��65��$
��Fill=�$	��115��$ 52��$	 217��$-��'/7W;component/bin/Debug/Images/quit.png���� -8   $��
helpButton��$��30��$
��Left=�$��774,414,0,0q�$	��Top=�$��29��$��2��$y�Hidden=����� $+ %/7W;component/Images/questionMark.png������ $3 -/7W;component/bin/Debug/Images/indexbg4x3.jpg��