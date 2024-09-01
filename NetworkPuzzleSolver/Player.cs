using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace NetworkPuzzleSolver
{
    public class Player
    {
        OpenQA.Selenium.Chrome.ChromeDriver driver;
        public int id = 0;
        int port = 9222;
        public static bool FromBitmap(Bitmap bmp, Size size, out Board b) {

            float cellW = (float)bmp.Width / (float)size.Width;
            float cellH = (float)bmp.Height / (float)size.Height;
            float edge = 0.4f; // 0.1 to .5
            Cell[] cells = new Cell [size.Width * size.Height];

            for (int i = 0; i < size.Width * size.Height; i++)
            {
                int row = i / size.Width;
                int col = i % size.Height;
                int centerX = (int)(col * cellW + cellW / 2);
                int centerY = (int)(row * cellH + cellH / 2);

                // North
                for (int n = centerX - (int)(cellW * edge); n < centerX + (int)(cellW * edge); n++)
                {
                    if (bmp.GetPixel(n, centerY - (int)(cellH * edge)).R < 200) //Pixel is black
                    {
                        cells[i].CanN = true;
                    }
                    //bmp.SetPixel(n, centerY - (int)(cellH * edge), Color.Red);
                }

                // South
                for (int n = centerX - (int)(cellW * edge); n < centerX + (int)(cellW * edge); n++)
                {
                    if (bmp.GetPixel(n, centerY + (int)(cellH * edge)).R < 200) //Pixel is black
                    {
                        cells[i].CanS = true;
                    }
                    //bmp.SetPixel(n, centerY + (int)(cellH * edge), Color.Red);
                }

                // East
                for (int n = centerY - (int)(cellH * edge); n < centerY + (int)(cellH * edge); n++)
                {
                    if (bmp.GetPixel(centerX + (int)(cellW * edge), n).R < 200) //Pixel is black
                    {
                        cells[i].CanE = true;
                    }
                    //bmp.SetPixel(centerX + (int)(cellW * edge), n, Color.Red);
                }

                // Weast
                for (int n = centerY - (int)(cellH * edge); n < centerY + (int)(cellH * edge); n++)
                {
                    if (bmp.GetPixel(centerX - (int)(cellW * edge), n).R < 200) //Pixel is Dark
                    {
                        cells[i].CanW = true;
                    }
                    //bmp.SetPixel(centerX - (int)(cellW * edge), n, Color.Red);
                }
            }
            b = new(cells, size);
            return true;
        }


        public void Start()
        {
            port = 9222 + id;
            string executableDirectory = AppContext.BaseDirectory;
            Console.WriteLine("Executable Directory: " + executableDirectory);
            // Specify the path to your custom user data directory
            string userDataDir = executableDirectory + $"\\Session";
            CopyDirectory(userDataDir, userDataDir + port.ToString());

            // Set up Chrome options to use the custom user data directory
            ChromeOptions options = new ChromeOptions();
            options.AddArgument($"--user-data-dir={userDataDir}{port}"); 
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            //options.AddArgument("--disable-extensions");

            if (id != 0)
                options.AddArgument("--headless");

            options.AddArgument($"--remote-debugging-port={port}"); // Ensure different debugging port
            //options.AddArgument($"--user-data-dir=C:/Temp/ChromeProfile1"); // Use different user profiles

            // Initialize the Chrome WebDriver with the options
            driver = new ChromeDriver(options);

            try
            {
                // Navigate to the webpage
                driver.Navigate().GoToUrl("https://puzzlemadness.co.uk/network/");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                // Close the browser
                driver.Quit();
            }
        }
        public void Run()
        {
            try
            {
                while (true)
                {

                    // Scroll down by a certain amount (e.g., 1000 pixels)
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;



                    // Find the button by class name and text
                    try
                    {
                        var button2 = driver.FindElement(By.Id("js-play-again-button"));
                        if (button2 != null)
                            button2.Click();
                    }
                    catch
                    { 
                        //look, its empty !
                    }

                    // Find the element to click (by ID in this example)
                    IWebElement element = driver.FindElement(By.Id("js-puzzle-target"));

                    // Get the vertical scroll position
                    long scrollTop = (long)js.ExecuteScript("return window.pageYOffset;");

                    // Scroll down by a certain amount (e.g., 1000 pixels)
                    js.ExecuteScript($"window.scrollBy(0, {element.Location.Y - 5 - (int)scrollTop});");


                    // Get the vertical scroll position
                    scrollTop = (long)js.ExecuteScript("return window.pageYOffset;");


                    Point loc = element.Location;
                    loc.Y -= (int)scrollTop;
                    Rectangle rect = new Rectangle(loc, element.Size);
                    var img = GetElementScreenShot(element, rect);
                    //img.Save("test.png", ImageFormat.Png);


                    for (int n = 6; n < 15; n++)
                    {

                        Size size = new Size(n, n);
                        if (Solve(size, img, out Board b))
                        {
                            Play(b, element, rect);
                            break;
                        }
                    }

                    Thread.Sleep(400);

                    // Find the button by class name and text
                    try
                    {
                        // Find the button by class name and text
                        var button = driver.FindElement(By.XPath("//div[@class='generic-button' and text()='Next puzzle']"));

                        // Click the button
                        
                        button.Click();

                        Console.WriteLine($"{id} - Up to next !");
                    }
                    catch
                    {
                        Console.WriteLine($"{id} - Cannot find btt");
                        // Find the button by class name and text
                        try
                        {
                            // Find the button by class name and text
                            var buttonR = driver.FindElement(By.Id("reset-button"));

                            // Click the button
                            buttonR.Click();
                        }
                        catch
                        {
                            Console.WriteLine($"{id} - Cannot find reset button");
                        }
                    }
                    Thread.Sleep(400);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                // Close the browser
                driver.Quit();
            }
        }
        bool Solve(Size size, Bitmap bitmap, out Board board)
        {
            if (!Player.FromBitmap(bitmap, size, out Board b))
            {
                board = null;
                return false;
            }
            board = b;
            //bitmap.Save($"{size}.png");
            //board.Draw().Save($"{size}.png");
            // Console.WriteLine("Scanned Board" + board.ToString());

            bool solved = board.Solve();

            return solved;
        }
        void Play(Board board, IWebElement element, Rectangle rect)
        {
            //Create an instance of Actions class
            Actions actions = new Actions(driver, TimeSpan.FromMilliseconds(200));
            Size size = board.size;
            for (int i = 0; i < size.Width * size.Height; i++)
            {
                int row = i / size.Width;
                int col = i % size.Height;

                int cellW = (int)rect.Size.Width / size.Width;
                int cellH = (int)rect.Size.Height / size.Height;

                int x = (int)rect.Location.X + cellW * col + cellW / 2;
                int y = (int)rect.Location.Y + cellH * row + cellH / 2;

                for (int n = 0; n < board.cells[i].RotateCount; n++)
                {
                    actions.MoveToLocation(x, y).Click();
                    //Thread.Sleep(5);
                }
            }
            actions.Perform();
        }
        public Bitmap GetElementScreenShot(IWebElement element, Rectangle rect)
        {
            Screenshot myScreenShot = ((ITakesScreenshot)driver).GetScreenshot();
            using (var screenBmp = new Bitmap(new MemoryStream(myScreenShot.AsByteArray)))
            {
                return screenBmp.Clone(rect, screenBmp.PixelFormat);
            }
        }
        public void Destroy()
        {
            driver.Quit();
        }

        static void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir2 = new DirectoryInfo(destinationDir);

            if (!dir.Exists)
            {
                return;
            }
            if (dir2.Exists)
            {
                return;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            Directory.CreateDirectory(destinationDir);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(tempPath, false);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destinationDir, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
        }
    }
}
