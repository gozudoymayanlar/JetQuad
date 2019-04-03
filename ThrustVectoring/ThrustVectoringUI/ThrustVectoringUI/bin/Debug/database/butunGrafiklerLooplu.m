clear all
clc

% yalýn verilerin tanýmlanmasý
run('C:\Users\NIGHTHAWK\Desktop\deneyButunVerilerYalin.m')

myTitleFontsize = 13;
myLblFontsize = 10;

% index tanýmlamalarý
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
% Verilerin birimlerini düzenleme
veriler = {veri2019_02_15__18_37_03;};
verilerIsimler = ['veri2019_02_15__18_37_03'];

    % ----------------
    % DENEYLERÝN VERÝLERÝNDE ÖZEL DÜZELTME VARSA BURAYA YAZILABÝLÝR
    % ÖRNEÐÝN GEREKSÝZ DATALARIN TEMÝZLENMESÝ, DENEYÝN ÖNCESÝ VE SONRASININ
    % TIRAÞLANMASI GÝBÝ...
    % ----------------

for i=1:size(veriler,1)
    %Time datasýnýn 0dan baþlamasý için her elemandan ilk zaman datasýný çýkarma
    veriler{i}(:,time) = veriler{i}(:,time) - veriler{1}(1,1);
    % zaman ms -> sn düzeltme [sn]
    veriler{i}(:,time) = veriler{i}(:,time)/1000;
    
    % ----------------
    % HER DENEYÝN VERÝLERÝNDE YAPILACAK BAÞKA DÜZELTME VARSA BURAYA YAZILABÝLÝR
    % ----------------
end


%%

close % açýk figürleri kapa

m = 2; % satir sayisi
n = 4; % sütün sayisi
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
                     % böylece birinde zoom yapýnca diðerlerinde de zoom yapýlýyor.
end



