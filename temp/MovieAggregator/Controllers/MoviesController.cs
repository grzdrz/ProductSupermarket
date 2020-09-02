﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MovieAggregator.Models;
using System.IO;

namespace MovieAggregator.Controllers
{
    public class MoviesController : Controller
    {
        private DBContextMoviesInfo db;

        public MoviesController()
        {
            db = new DBContextMoviesInfo();
            db.Configuration.ProxyCreationEnabled = false;
        }

        public async Task<JsonResult> Details(int? id)
        {
            if (id == null)
            {
                return Json(new { isDataReceivedSuccessfully = false }, JsonRequestBehavior.AllowGet);
            }

            Movie movie = await db.Movies.Include(a => a.Cast).Include(p => p.Producers).FirstAsync(m => m.Id == id);
            //обнуление циклических ссылок, иначе жисон ругается
            foreach (var c in movie.Cast)
            {
                c.Movies = null;
            }
            foreach (var p in movie.Producers)
            {
                p.Movies = null;
            }

            if (movie == null)
            {
                return Json(new { isDataReceivedSuccessfully = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(movie, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DependentDetails()
        {
            IEnumerable<Actor> cast = await db.Cast.ToListAsync();
            IEnumerable<Producer> producers = await db.Producers.ToListAsync();

            return Json(new { 
                cast = cast,
                producers = producers
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(Movie movie, int[] selectedActors, int[] selectedProducers, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                    using (BinaryReader br = new BinaryReader(image.InputStream))
                    {
                        var path = "C:\\Users\\space\\Music\\MovieAggregator\\MovieAggregator\\Content\\Images\\" + image.FileName;
                        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            await fs.WriteAsync(br.ReadBytes(image.ContentLength), 0, image.ContentLength);
                        }
                    }

                db.Movies.Add(movie);
                if (selectedActors != null)
                {
                    foreach (var actor in db.Cast.Where(a => selectedActors.Contains(a.Id)))
                    {
                        movie.Cast.Add(actor);
                    }
                }
                if (selectedProducers != null)
                {
                    foreach (var producer in db.Producers.Where(p => selectedProducers.Contains(p.Id)))
                    {
                        movie.Producers.Add(producer);
                    }
                }

                await db.SaveChangesAsync();
                return Json(new { isDataReceivedSuccessfully = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isDataReceivedSuccessfully = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Edit(Movie movie, int[] selectedActors, int[] selectedProducers, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                    using (BinaryReader br = new BinaryReader(image.InputStream))
                    {
                        var path = "C:\\Users\\space\\Music\\MovieAggregator\\MovieAggregator\\Content\\Images\\" + image.FileName;
                        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            await fs.WriteAsync(br.ReadBytes(image.ContentLength), 0, image.ContentLength);
                        }
                    }


                Movie movieToReplace = db.Movies.FirstOrDefault(m => m.Id == movie.Id);
                db.Movies.Remove(movieToReplace);
                await db.SaveChangesAsync();


                db.Movies.Add(movie);
                if (selectedActors != null)
                {
                    foreach (var actor in db.Cast.Where(a => selectedActors.Contains(a.Id)))
                    {
                        movie.Cast.Add(actor);
                    }
                }
                if (selectedProducers != null)
                {
                    foreach (var producer in db.Producers.Where(p => selectedProducers.Contains(p.Id)))
                    {
                        movie.Producers.Add(producer);
                    }
                }
                await db.SaveChangesAsync();
                return Json(new { isDataReceivedSuccessfully = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { isDataReceivedSuccessfully = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            Movie movie = await db.Movies.FindAsync(id);
            db.Movies.Remove(movie);
            await db.SaveChangesAsync();
            return Json(new { isDataReceivedSuccessfully = true }, JsonRequestBehavior.AllowGet);
        }


        public string GetMoviesCount()
        {
            int moviesCount = db.Movies.Count();

            return moviesCount.ToString();
        }

        public string GetMoviesInfoPageCount()
        {
            int moviesCount = db.Movies.Count();

            return ((int)Math.Ceiling((double)moviesCount / 4d)).ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}