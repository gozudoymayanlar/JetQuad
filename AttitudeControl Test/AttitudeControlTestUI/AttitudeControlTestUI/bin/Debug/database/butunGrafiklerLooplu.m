clear all
clc

% yal�n verilerin tan�mlanmas�
run('C:\Users\NIGHTHAWK\Desktop\deneyButunVerilerYalin.m')

myTitleFontsize = 13;
myLblFontsize = 10;

% index tan�mlamalar�
time = 1;
MotorStatus = 2;
KumandaThrottle = 3;
KumandaTrim = 4;
ArduThrottle = 5;
ArduTrim = 6;
MotorThrottle = 7;
Thrust = 8;
RefRPM = 9;
RPM = 10;
EGT = 11;
BatteryVoltage = 12;
PumpVoltage = 13;
Fuel = 14;


%%
% Verilerin birimlerini d�zenleme
veriler = {veri2019_02_15__18_37_03;};
verilerIsimler = ['veri2019_02_15__18_37_03'];

    % ----------------
    % DENEYLER�N VER�LER�NDE �ZEL D�ZELTME VARSA BURAYA YAZILAB�L�R
    % �RNE��N GEREKS�Z DATALARIN TEM�ZLENMES�, DENEY�N �NCES� VE SONRASININ
    % TIRA�LANMASI G�B�...
    % ----------------

for i=1:size(veriler,1)
    %Time datas�n�n 0dan ba�lamas� i�in her elemandan ilk zaman datas�n� ��karma
    veriler{i}(:,time) = veriler{i}(:,time) - veriler{1}(1,1);
    % zaman ms -> sn d�zeltme [sn]
    veriler{i}(:,time) = veriler{i}(:,time)/1000;
    
    % ----------------
    % HER DENEY�N VER�LER�NDE YAPILACAK BA�KA D�ZELTME VARSA BURAYA YAZILAB�L�R
    % ----------------
end


%%

close % a��k fig�rleri kapa

m = 2; % satir sayisi
n = 4; % s�t�n sayisi
myTitleFontsize = 13;
myLblFontsize = 10;

%     plot(seconds(veriler{i}(:,time)), veriler{i}(:,MotorStatus), 'r-', 'DurationTickFormat','mm:ss')
%     title('Durum', 'Interpreter', 'latex', 'fontsize', myTitleFontsize);
%     xlabel('Zaman [sn]', 'fontsize', myLblFontsize)
%     legend({'Durum'}, 'Interpreter', 'latex')
%     axis tight

% 'DurationTickFormat','mm:ss'
% seconds(veriler{i}(:,zaman))


for i=1:size(veriler,1)
    p = 1; % plot indexi
    figure('NumberTitle', 'off', 'Name', verilerIsimler(i), 'color', 'white')
    %1
    sp(p) = subplot(m,n,p);
    plot((veriler{i}(:,time)), veriler{i}(:,KumandaThrottle), 'r-',...
        (veriler{i}(:,time)), veriler{i}(:,ArduThrottle), 'b--')
    title('Throttle', 'fontsize', myTitleFontsize);
    xlabel('Zaman [sn]', 'fontsize', myLblFontsize)
    ylabel('%', 'fontsize', myLblFontsize)
    legend({'KumandaThrottle', 'ArduThrottle'})
    axis tight
    p = p+1;
    
    %2
    sp(p) = subplot(m,n,p);
    plot((veriler{i}(:,time)), veriler{i}(:,KumandaTrim), 'r-',...
        (veriler{i}(:,time)), veriler{i}(:,ArduTrim), 'b--')
    title('Trim', 'fontsize', myTitleFontsize);
    xlabel('Zaman [sn]', 'fontsize', myLblFontsize)
    ylabel('%', 'fontsize', myLblFontsize)
    legend({'KumandaTrim', 'ArduTrim'})
    axis tight
    p = p+1;
    
    %3
    sp(p) = subplot(m,n,p);
    plot((veriler{i}(:,time)), veriler{i}(:,MotorThrottle), 'r-')
    title('MotorThrottle', 'fontsize', myTitleFontsize)
    xlabel('Zaman [sn]', 'fontsize', myLblFontsize)
    ylabel('%', 'fontsize', myLblFontsize)
    legend({'MotorThrottle'})
    axis tight
    p = p+1;
    
    %4
    sp(p) = subplot(m,n,p);
    plot((veriler{i}(:,time)), veriler{i}(:,RPM), 'r-')
    title('RPM', 'fontsize', myTitleFontsize);
    xlabel('Zaman [sn]', 'fontsize', myLblFontsize)
    ylabel('RPM', 'fontsize', myLblFontsize)
    legend({'RPM'})
    axis tight
    p = p+1;
    
    %5
    sp(p) = subplot(m,n,p);
    plot((veriler{i}(:,time)), veriler{i}(:,EGT), 'r-')
    title('EGT', 'fontsize', myTitleFontsize);
    xlabel('Zaman [sn]', 'fontsize', myLblFontsize)
    ylabel('EGT [$\circ$ C]','Interpreter', 'latex', 'fontsize', myLblFontsize)
    legend({'EGT'})
    axis tight
    p = p+1;
    
    %6
    sp(p) = subplot(m,n,p);
    plot((veriler{i}(:,time)), veriler{i}(:,BatteryVoltage), 'r-')
    title('BatteryVoltage', 'fontsize', myTitleFontsize);
    xlabel('Zaman [sn]', 'fontsize', myLblFontsize)
    ylabel('Voltage [V]', 'fontsize', myLblFontsize)
    legend({'BatteryVoltage'})
    axis tight
    p = p+1;
    
    %7
    sp(p) = subplot(m,n,p);
    plot((veriler{i}(:,time)), veriler{i}(:,PumpVoltage), 'r-')
    title('PumpVoltage', 'fontsize', myTitleFontsize);
    xlabel('Zaman [sn]', 'fontsize', myLblFontsize)
    ylabel('Voltage [V]', 'fontsize', myLblFontsize)
    legend({'PumpVoltage'})
    axis tight
    p = p+1;
    
    %8
    sp(p) = subplot(m,n,p);
    plot((veriler{i}(:,time)), veriler{i}(:,Fuel), 'r-')
    title('Fuel', 'fontsize', myTitleFontsize);
    xlabel('Zaman [sn]', 'fontsize', myLblFontsize)
    ylabel('Mass [kg]', 'fontsize', myLblFontsize)
    legend({'Fuel'})
    axis tight
    p = p+1;
    
    linkaxes(sp,'x') % eksenleri birbirine linkliyor.
                     % b�ylece birinde zoom yap�nca di�erlerinde de zoom yap�l�yor.
end



