   M S B A M L   `   `   `  ����I  EJetMotorTestUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:JetMotorTestUIJetMotorTestUI  5   	       JetMotorTestUI.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:JetMotorTestUI     5   	   \ XArction.WPF.Gauges, Version=1.0.0.4001, Culture=neutral, PublicKeyToken=6484d7bb14b95dd3(agchttp://www.arction.com/gauges/  5   	     � Title$  JetMotorTestUI��5	   	   $	��768��6    $
��1300��6-    � Icon$ Resources/jetTurbine.png��6:   ���	#FFDEDEDE.��5
      � .+�5   
     Arction.Gauges.Gauge  $	��249��6   $	��315��6!   $��5,5,0,0q�6-   $
��Left=�6>   $	��Top=�6Y      
AngleBegin$	 190Q�5         AngleEnd$	 -10Q�6%      AdjacentPadding$ 5Q�64      Fill
 ����    
ArcPadding$ 0Q�6[      PrimaryScale 5        Arction.Gauges.Scale  5      $��
0,0,0,-135q�6      AdjacentPadding$	 100Q�60   	   
AngleBegin$		 180Q�5      
   AngleEnd$
 0Q�6-      RangeEnd$	 150Q�6:      
DialColor1$ 	#FF3C3D95��5         
DialColor2$	 Red��63      
DialColor3$ 	#FFD2AF64��6D      
LabelAngle$	 225Q�5         LabelDistance$ 1Q�6-      MajorTickCount$ 6��5         MinorTickCount$ 2��6/      ValueIndicatorAngle$	 315Q�5         ValueIndicatorDistance$ 1Q�66      ValueFormat$ F3��6Q      
ArcPadding$ 0Q�6b      Value$ 64Q�6q   $��20(�6|      Dial 5        Arction.Gauges.Dials.Dial  5         	DialShape$ 
WideNeedle=�6#      Label 5      �� $��RPM (x1000)��6$   $��22(�67   5         SecondaryScales 5        $		 180Q�6   $	
 144Q�6-   $ 55Q�6<      ArcStrokeThickness$ 20Q�6L      	ArcStroke
 ���@�6d     5      $		 144Q�6   $	
 108Q�6-   $ 55Q�6<   $ 20Q�6L    �#3C3D956d     5       $		 108Q�6   $
 72Q�6-   $ 55Q�6;   $ 20Q�6K    �	#CCFFFF006c     5!      $	 72Q�6   $
 24Q�6,   $ 55Q�6:   $ 20Q�6J   
 � ���6b     5"      $	 24Q�6   $
 0Q�6,   $ 55Q�69   $ 20Q�6I    �	#CCFF00006a   5$      5%      5&      