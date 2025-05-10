//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using FilmLibraryManagement.Data;
//using FilmLibraryManagement.Models;
//using Microsoft.EntityFrameworkCore;
//using OfficeOpenXml;

//public class ExcelDataImporter
//{
//    private readonly DataContext _context;

//    public ExcelDataImporter(DataContext context)
//    {
//        _context = context;
//    }

//    public async Task ImportExcelData(string filePath)
//    {
//        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
//        FileInfo fileInfo = new FileInfo(filePath);

//        using (var package = new ExcelPackage(fileInfo))
//        {
//            var worksheet = package.Workbook.Worksheets[0];
//            int rowCount = worksheet.Dimension.Rows;

//            Console.WriteLine($"✅ Excel file found. Starting import...");
//            Console.WriteLine($"📌 Total rows found (including header): {rowCount}");

//            for (int row = 2; row <= rowCount; row++) // Start from row 2 (skip headers)
//            {
//                try
//                {
//                    var cells = worksheet.Cells[row, 1, row, worksheet.Dimension.Columns];

//                    // **Read Data From Correct Columns**
//                    string title = cells[1].Text.Trim(); // Column A (Title)
//                    if (string.IsNullOrWhiteSpace(title))
//                    {
//                        Console.WriteLine($"⚠️ Skipping row {row}: Empty title");
//                        continue;
//                    }

//                    if (!int.TryParse(cells[2].Text.Trim(), out int releaseYear)) // Column B (Release Year)
//                    {
//                        Console.WriteLine($"⚠️ Skipping row {row}: Invalid release year '{cells[2].Text}'");
//                        continue;
//                    }

//                    string description = cells[3].Text.Trim(); // Column C (Description)

//                    if (!double.TryParse(cells[4].Text.Trim(), out double rating)) // Column D (Rating)
//                    {
//                        Console.WriteLine($"⚠️ Skipping row {row}: Invalid rating '{cells[4].Text}'");
//                        continue;
//                    }

//                    string directorsString = cells[5].Text.Trim(); // Column E (Directors)
//                    string genresString = cells[6].Text.Trim(); // Column F (Genres)

//                    // **Check if movie already exists (avoid duplicates)**
//                    var movie = await _context.Movies
//                        .Include(m => m.MovieDirectors)
//                        .Include(m => m.MovieGenres)
//                        .FirstOrDefaultAsync(m => m.Title == title && m.ReleaseYear == releaseYear);

//                    if (movie == null)
//                    {
//                        movie = new Movie
//                        {
//                            Title = title,
//                            ReleaseYear = releaseYear,
//                            Description = description,
//                            Rating = rating,
//                            MovieDirectors = new List<MovieDirector>(),
//                            MovieGenres = new List<MovieGenre>()
//                        };

//                        _context.Movies.Add(movie);
//                    }

//                    // **Process directors**
//                    var directors = directorsString.Split(",").Select(d => d.Trim()).Where(d => !string.IsNullOrEmpty(d)).ToList();
//                    foreach (var directorName in directors)
//                    {
//                        var director = await _context.Directors.FirstOrDefaultAsync(d => d.Name == directorName);
//                        if (director == null)
//                        {
//                            director = new Director { Name = directorName };
//                            _context.Directors.Add(director);
//                            await _context.SaveChangesAsync(); // Save immediately to get ID
//                        }

//                        if (!movie.MovieDirectors.Any(md => md.DirectorId == director.Id))
//                        {
//                            movie.MovieDirectors.Add(new MovieDirector { Director = director });
//                        }
//                    }

//                    // **Process genres**
//                    var genres = genresString.Split(",").Select(g => g.Trim()).Where(g => !string.IsNullOrEmpty(g)).ToList();
//                    foreach (var genreName in genres)
//                    {
//                        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
//                        if (genre == null)
//                        {
//                            genre = new Genre { Name = genreName };
//                            _context.Genres.Add(genre);
//                            await _context.SaveChangesAsync(); // Save immediately to get ID
//                        }

//                        if (!movie.MovieGenres.Any(mg => mg.GenreId == genre.Id))
//                        {
//                            movie.MovieGenres.Add(new MovieGenre { Genre = genre });
//                        }
//                    }

//                    await _context.SaveChangesAsync(); // Save movie and relations
//                    Console.WriteLine($"✅ Imported row {row}: {title} ({releaseYear})");
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"❌ Error processing row {row}: {ex.Message}");
//                }
//            }

//            Console.WriteLine("🎉 Import completed!");
//        }
//    }
//}
