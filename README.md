# cr
Copy RAW. Command line app to copy RAW files from a camera's memory card to disk.

To build, use your favorite version of .net:

    c:\windows\microsoft.net\framework64\v4.0.30319\csc.exe cr.cs /nologo /D:_WINDOWS /nowarn:0168
    
Or, use the m.sh and cr.csproj to build with net6.0 on MacOS. This will produce a CR binary in the same folder.
    
Usage:    

    Usage: cr [-d:N] [-e:XYZ] [-j] <sourcepath> <destinationpath>
    Copy Raw. Copies RAW files from a camera's memory card to a disk
      arguments:  [-d:N]   Copy images from the last N days they were taken. Default is 1.
                  [-e:XXX] Copies a specific extension. (with or without a period)
                  [-g]     Don't copy; just show image counts grouped by day + year.
                  [-j]     Copy .JPG files as well as RAW files if they exist.
                  [-s]     Show the files that would be copied, but don't actually copy.
      example:    cr f:\ .
                  cr f:\dcim\100canon d:\pics\oct29_20
                  cr -j f:\dcim d:\pics\oct29_20
                  cr -d:3 f:\ .
                  cr -d:2 -e:CR2 f:\ .
                  cr -e:.NEF f:\dcim .
                  cr e: -g
                  cr g
      on MacOS:   cr /Volumes/OLY\ E-M10II .
      notes:
        Copies .3FR .ARW, .CR2, .CR3, .DNG, .NEF, .ORF, .RAF, .RW2 files by default. Override with -e
        The -d:N argument isn't a count of days from the current day. It's the number of days
          photos were taken on the camera. For example, if today is November 1st and you last took
          photos on October 28th, 25th, 23rd, and 10th, -d:3 copies images from the most recent
          three days. To just get photos from the 28th, use -d:1 or don't specify -d since the
          default is 1.
        Files with identical names and older or equal last write times (previously copied) are skipped.
        If lightroom has written metadata to .DNG files on disk, those won't be over-written.
        Use -g to get a breakdown of what -d:N value to use if you can't remember when you took photos.
        If <destinationpath> isn't specified, -g is assumed.
        On Windows, use either - or / for arguments. On MacOS, only - is supported.
        
Sample Usage: D:\>cr g:

    Date                             Days  Photos                 Size
        Saturday, November 5, 2022      1      34        2,956,260,352
        Saturday, October 29, 2022      2       7          604,538,880
         Tuesday, October 25, 2022      3      11          969,393,664
           Tuesday, August 2, 2022      4      11          964,412,416
            Monday, August 1, 2022      5      40        3,451,437,568
           Saturday, July 30, 2022      6      51        4,413,136,384
            Tuesday, June 21, 2022      7       2          171,849,216
              Sunday, May 22, 2022      8       3          259,931,136
           Wednesday, May 11, 2022      9       2          175,177,728
