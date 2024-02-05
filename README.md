# Export EdooonGPS app Track Record

EdooonGPS has shutdown their service in Nov. 2017.After long time , I've eliminated three smartphones, I still have that app on my last phone!

First you need backup your EdoonGPS app data from you phone.
like  `adb backup -f D:\edooon.ab Edooon.ab com.edooon.gps` comand or use MIUI backup function(if you use XiaoMi or Redmi).


Then extart or decrypt from `java -jar abe.jar D:\edooon.ab D:\aaa.tar`.
you will found the `Cearch.db` in db Folder.That is a Sqlite db file.

copy Cearch.db to the program which name .exe folder,double cliek the exe.
it will created all track list in Edooon app,which has track point record.

also support conver wgs coordinate to gcj-02 coordinate,it only use for chinese area.

# Reference

[C C# Python ...](https://github.com/googollee/eviltransform)
[github first coordinate convert version](https://github.com/Leask/EvilTransform)
[Dapper](https://www.nuget.org/packages/Dapper/)
[Geo](https://www.nuget.org/packages/Geo)