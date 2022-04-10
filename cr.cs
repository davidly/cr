using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class CopyRaw
{
    static void Usage()
    {
        Console.WriteLine( @"Usage: cr [-d:N] [-e:XYZ] [-j] <sourcepath> <destinationpath>" );
        Console.WriteLine( @"Copy Raw. Copies RAW files from a camera's memory card to a disk" );
        Console.WriteLine( @"  arguments:  [-d:N]   Copy images from the last N days they were taken. Default is 1." );
        Console.WriteLine( @"              [-e:XXX] Copies a specific extension. (with or without a period)" );
        Console.WriteLine( @"              [-g]     Don't copy; just show image counts grouped by day + year." );
        Console.WriteLine( @"              [-j]     Copy .JPG files as well as RAW files if they exist." );
        Console.WriteLine( @"              [-s]     Show the files that would be copied, but don't actually copy." );
        Console.WriteLine( @"  example:    cr f:\ ." );
        Console.WriteLine( @"              cr f:\dcim\100canon d:\pics\oct29_20" );
        Console.WriteLine( @"              cr -j f:\dcim d:\pics\oct29_20" );
        Console.WriteLine( @"              cr -d:3 f:\ ." );
        Console.WriteLine( @"              cr -d:2 -e:CR2 f:\ ." );
        Console.WriteLine( @"              cr -e:.NEF f:\dcim ." );
        Console.WriteLine( @"              cr e: -g" );
        Console.WriteLine( @"  notes:" );
        Console.WriteLine( @"    Copies .3FR .ARW, .CR2, .CR3, .DNG, .NEF, .ORF, .RAF, .RW2 files by default. Override with -e" );
        Console.WriteLine( @"    The -d:N argument isn't a count of days from the current day. It's the number of days" );
        Console.WriteLine( @"      photos were taken on the camera. For example, if today is November 1st and you last took" );
        Console.WriteLine( @"      photos on October 28th, 25th, 23rd, and 10th, -d:3 copies images from the most recent" );
        Console.WriteLine( @"      three days. To just get photos from the 28th, use -d:1 or don't specify -d since the" );
        Console.WriteLine( @"      default is 1." );
        Console.WriteLine( @"    Files with identical names and older or equal last write times (previously copied) are skipped." );
        Console.WriteLine( @"    If lightroom has written metadata to .DNG files on disk, those won't be over-written." );
        Console.WriteLine( @"    Use -g to get a breakdown of what -d:N value to use if you can't remember when you took photos." );
        Console.WriteLine( @"    If <destinationpath> isn't specified, -g is assumed." );

        Environment.Exit( 1 );
    } //Usage

    public class DayYear
    {
        public int day;
        public int year;
        public int count;
        public string datestring;

        public DayYear( int d, int y )
        {
            day = d;
            year = y;
            count = 0;
        } //DayYear
    } //DayYear

    static bool CopyImageFile( string fullPath, string dstRoot, bool showDoNotCopy )
    {
        bool copied = false;

        try
        {
            string name = Path.GetFileName( fullPath );
            string dstPath = Path.Combine( dstRoot, name );
    
            DateTime dtSrc = File.GetLastWriteTime( fullPath ); 
            DateTime dtDst = File.GetLastWriteTime( dstPath );

            if ( DateTime.Compare( dtSrc, dtDst ) > 0 )
            {
                Console.WriteLine( "copying from {0} to {1}", fullPath, dstPath );

                if ( ! showDoNotCopy )
                    File.Copy( fullPath, dstPath, true );

                copied = true;
            }
        }
        catch ( Exception ex)
        {
            Console.WriteLine( "exception {0} caught copying {1} to {2}", ex.ToString(), fullPath, dstRoot );
        }

        return copied;
    } //CopyImageFile

    static void Main( string[] args )
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string srcRoot = null;
        string dstRoot = null;
        string specificExtension = null;
        bool copyJPGToo = false;
        bool showDoNotCopy = false;
        bool showGrouping = false;
        int days = 1;

        for ( int i = 0; i < args.Length; i++ )
        {
            if ( ( '-' == args[i][0] )
#if _WINDOWS
                 || ( '/' == args[i][0] )
#endif
               )
            {
                string arg = args[i];
                string argUpper = arg.ToUpper();
                char c = argUpper[1];
    
                if ( 'D' == c )
                {
                    if ( ( arg.Length < 4 ) || ( ':' != arg[2] ) )
                        Usage();

                    days = Convert.ToInt32( arg.Substring( 3 ) );

                    if ( 0 == days )
                        Usage();
                }
                else if ( 'E' == c )
                {
                    if ( arg.Length < 4 )
                         Usage();

                    if ( ':' != arg[2] )
                        Usage();

                    specificExtension = arg.Substring( 3 );
                    if ( '.' != specificExtension[0] )
                        specificExtension = "." + specificExtension;
                }
                else if ( 'G' == c )
                {
                    if ( arg.Length > 2 )
                         Usage();

                    showGrouping = true;
                }
                else if ( 'J' == c )
                {
                    if ( arg.Length > 2 )
                         Usage();

                    copyJPGToo = true;
                }
                else if ( 'S' == c )
                {
                    if ( arg.Length > 2 )
                         Usage();

                    showDoNotCopy = true;
                }
                else
                    Usage();
            }
            else
            {
                if ( null == srcRoot )
                    srcRoot = args[i];
                else if ( null == dstRoot )
                    dstRoot = args[i];
                else
                    Usage();
            }
        }

        if ( null == dstRoot )
            showGrouping = true;

        if ( ( null == srcRoot ) || ( ( null == dstRoot ) && !showGrouping ) )
            Usage();

        srcRoot = Path.GetFullPath( srcRoot );

        if ( null != dstRoot )
        {
            dstRoot = Path.GetFullPath( dstRoot );
            Console.WriteLine( "Copying from {0} to {1}", srcRoot, dstRoot );
        }

        SortedSet<string> copiedExtensions = new SortedSet<string>( new ByStringCompare() );

        if ( null != specificExtension )
            copiedExtensions.Add( specificExtension );
        else
        {
            copiedExtensions.Add( ".3fr" );
            copiedExtensions.Add( ".arw" );
            copiedExtensions.Add( ".cr2" );
            copiedExtensions.Add( ".cr3" );
            copiedExtensions.Add( ".dng" );
            copiedExtensions.Add( ".nef" );
            copiedExtensions.Add( ".orf" );
            copiedExtensions.Add( ".raf" );
            copiedExtensions.Add( ".rw2" );
        }

        if ( copyJPGToo )
            copiedExtensions.Add( ".jpg" );

        long bytesCopied = 0;
        long filesCopied = 0;
        long filesExamined = 0;
        string extension = "*.*";

        // First enumerate all the files and get the day/year groupings for them

        SortedSet<FileInfo> sourceFiles = new SortedSet<FileInfo>( new ByDateSortedSet() );
        SortedSet<DayYear> dySet = new SortedSet<DayYear>( new ByDayYearSortedSet() );

        foreach ( FileInfo fsiP in GetFilesInfo( srcRoot, extension ) )
        {
            FileInfo fi = (FileInfo) fsiP;

            try
            {
                string ext = Path.GetExtension( fi.Name );

                if ( copiedExtensions.Contains( ext ) )
                {
                    sourceFiles.Add( fi );

                    // DayOfYear is 1 to 366 (for leap years)

                    DayYear dy = new DayYear( fi.LastWriteTime.DayOfYear, fi.LastWriteTime.Year );
                    dy.datestring = fi.LastWriteTime.ToLongDateString();
                    dySet.Add( dy );
                }

                filesExamined++;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( "exception {0} caught", ex.ToString() );
            }
        }

        if ( showGrouping )
        {
            int daysback = 1;

            foreach ( FileInfo fi in sourceFiles )
            {
                DayYear dy = new DayYear( fi.LastWriteTime.DayOfYear, fi.LastWriteTime.Year );
                DayYear dyActual;

                if ( dySet.TryGetValue( dy, out dyActual ) )
                    dyActual.count++;
            }

           Console.WriteLine( "Date                             Days  Photos" );
           foreach ( DayYear dy in dySet )
               Console.WriteLine( "{0,30}   {1,4} {2,7}", dy.datestring, daysback++, dy.count );
//               Console.WriteLine( "{0,3}  {1}  {2,4} {3,7}", dy.day, dy.year, daysback++, dy.count );

            return;
        }

        // Get just the DayYears that match the # of days specified.

        SortedSet<DayYear> dyCopied = new SortedSet<DayYear>( new ByDayYearSortedSet() );

        foreach ( DayYear dy in dySet )
        {
            if ( dyCopied.Count < days )
                dyCopied.Add( dy );
            else
                break;
        }

        dySet.Clear();
        dySet = null;

        try
        {
            Directory.CreateDirectory( dstRoot );
        }
        catch ( Exception ex )
        {
            Console.WriteLine( "exception {0} creating directory {1}", ex.ToString(), dstRoot );
        }

        // Finally, copy the files.
        // Non-scientific measurements: 1 thread 81 MB/S, 2 threads 85.3 MB/S, 3 threads 86.7 MB/S, 4 threads 88.1 MB/S and close to 100% busy

        //foreach ( FileInfo fi in sourceFiles )
        Parallel.ForEach( sourceFiles, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (fi) =>
        {
            DayYear dy = new DayYear( fi.LastWriteTime.DayOfYear, fi.LastWriteTime.Year );

            if ( dyCopied.Contains( dy ) )
            {
                bool copied = CopyImageFile( fi.FullName, dstRoot, showDoNotCopy );

                if ( copied )
                {
                    Interlocked.Increment( ref filesCopied );
                    Interlocked.Add( ref bytesCopied, fi.Length );
                }
            }
        });

        Console.WriteLine( "Examined {0} files. {1} files matched extension. Copied {2} files consuming {3,0:N0} bytes", filesExamined, sourceFiles.Count, filesCopied, bytesCopied );

        if ( 0 != bytesCopied && 0 != stopWatch.ElapsedMilliseconds )
        {
            double bytesPerMillisecond = (double) bytesCopied / (double) stopWatch.ElapsedMilliseconds;
            double bytesPerSecond = bytesPerMillisecond * 1000.0;

            Console.WriteLine( "copy rate {0:0.##} MB/sec", bytesPerSecond / (double) ( 1024 * 1024 ) );
            if ( showDoNotCopy )
                Console.WriteLine( "NOTE: didn't really copy the files due to -s flag" );
        }
    } //Main

    public class ByStringCompare : IComparer<string>
    {
        CaseInsensitiveComparer comp = new CaseInsensitiveComparer();

        public int Compare( string x, string y )
        {
            return comp.Compare( x, y );
        }
    } //ByStringCompare

    static IEnumerable<FileInfo> GetFilesInfo( string path, string extension )
    {
        Queue<string> queue = new Queue<string>();
        queue.Enqueue( path );
    
        while ( queue.Count > 0 )
        {
            path = queue.Dequeue();
            try
            {
                // GetDirectories will not return any subdirectories under a reparse point

                foreach ( string subDir in Directory.GetDirectories( path ) )
                    queue.Enqueue( subDir );
            }
            catch(Exception ex)
            {
                //Console.Error.WriteLine( "Exception finding subdirectories {0} in {1}", ex.ToString(), path );
            }

            FileInfo[] files = null;
            try
            {
                DirectoryInfo di = new DirectoryInfo( path );
                files = di.GetFiles(  extension );
            }
            catch (Exception ex)
            {
                //Console.Error.WriteLine( "Exception finding .xmp files {0} in {1}", ex.ToString(), path );
            }
    
            if ( files != null )
            {
                for ( int i = 0 ; i < files.Length; i++ )
                {
                    yield return files[ i ];
                }
            }
        }
    } //GetFilesInfo

    public class ByDateSortedSet : IComparer<FileInfo>
    {
        CaseInsensitiveComparer comp = new CaseInsensitiveComparer();

        public int Compare( FileInfo fiA, FileInfo fiB )
        {
            if ( fiA.LastWriteTime != fiB.LastWriteTime )
            {
                if ( fiA.LastWriteTime > fiB.LastWriteTime )
                    return 1;

                return -1;
            }

            return comp.Compare( fiA.FullName, fiB.FullName );
        }
    } //ByDateSortedSet

    public class ByDayYearSortedSet : IComparer<DayYear>
    {
        public int Compare( DayYear a, DayYear b )
        {
            // Sort so most recent days are first

            if ( a.year != b.year )
                return b.year - a.year;

            return ( b.day - a.day );
        }
    } //ByDayYear
} //CopyRaw
