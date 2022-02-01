# cr
Copy RAW. Windows command line app to copy RAW files from a flash card used in a camera.

To build, use your favorite version of .net:

    c:\windows\microsoft.net\framework64\v4.0.30319\csc.exe cr.cs /nologo /nowarn:0168
    
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
                  cr /d:2 -e:CR2 f:\ .
                  cr /e:.NEF f:\dcim .
                  cr e: /g
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
