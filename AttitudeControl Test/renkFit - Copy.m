clc
clear all

colors = [255 0 0; 255 64 0; 255 128 0;255 191 0;255 255 0;191 255 0;...
    128 255 0;64 255 0;0 255 0;0 255 64;0 255 128;0 255 191;0 255 255];

% plot3(colors(:,1), colors(:,2), colors(:,3))

Red = colors(:,1);
Green = colors(:,2);
Blue = colors(:,3);

Sicaklik = transpose(60:-(60-10)/12:10);


a1 = 247.4  %(-589, 926.1)
b1 = 59.87  %(13.92, 41.52)
c1 = 14.66  %(1.615, 20.21)
a2 = 182.4  %(103.7, 382.9)
b2 = 42.14  %(0.06235, 18.13)
c2 = 9.916   %(-44.49, 76.42)
x = 20
f =  a1*exp(-((x-b1)/c1)^2) + a2*exp(-((x-b2)/c2)^2)



p1 =   -0.006167  %(-0.00897, -0.003364)
p2 =      0.4276  %(0.1309, 0.7242)
p3 =       -8.79  %(-18.26, 0.6796)
p4 =       307.7  %(219.6, 395.9)
       
f = p1*x^3 + p2*x^2 + p3*x + p4


p1 =   -0.006143  %(-0.008977, -0.003308)
p2 =       0.865  %(0.565, 1.165)
p3 =      -39.51  %(-49.09, -29.94)
p4 =       582.4  %(493.2, 671.5)

f = p1*x^3 + p2*x^2 + p3*x + p4