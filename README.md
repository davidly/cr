# cr
Copy RAW. Windows command line app to copy RAW files from a camera's memory card to disk.

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
        
Sample Usage: D:\>cr g:

    Date                             Days  Photos
            Sunday, March 20, 2022      1      51
         Tuesday, October 12, 2021      2       2
         Saturday, October 9, 2021      3      13
           Monday, October 4, 2021      4       2
        Sunday, September 19, 2021      5      41
         Saturday, August 28, 2021      6       2
           Saturday, June 19, 2021      7      62
           Sunday, August 23, 2020      8      11
           Saturday, July 25, 2020      9       2
             Sunday, June 21, 2020     10      47
        Tuesday, February 11, 2020     11      16
          Friday, January 24, 2020     12      55
          Friday, January 17, 2020     13       5
       Saturday, December 28, 2019     14       6
         Sunday, November 24, 2019     15      36
       Wednesday, October 30, 2019     16       2
         Saturday, August 31, 2019     17      57
         Thursday, August 22, 2019     18       8
          Tuesday, August 20, 2019     19       4
           Monday, August 19, 2019     20      89
         Saturday, August 17, 2019     21      94
           Friday, August 16, 2019     22      43
        Wednesday, August 14, 2019     23      13
         Wednesday, August 7, 2019     24      37
           Tuesday, August 6, 2019     25      63
            Monday, August 5, 2019     26      12
            Sunday, August 4, 2019     27      29
          Saturday, August 3, 2019     28      71
           Saturday, July 27, 2019     29      44
            Sunday, April 28, 2019     30       4
            Sunday, April 21, 2019     31      43
            Sunday, March 31, 2019     32      99
            Friday, March 29, 2019     33       7
          Thursday, March 28, 2019     34      23
          Saturday, March 23, 2019     35      28
            Friday, March 22, 2019     36       7
          Thursday, March 21, 2019     37      10
         Wednesday, March 20, 2019     38       7
           Tuesday, March 19, 2019     39       6
