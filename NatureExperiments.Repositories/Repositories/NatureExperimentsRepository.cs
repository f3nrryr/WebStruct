using Microsoft.EntityFrameworkCore;
using NatureExperiments.DAL.Models;
using NatureExperiments.Repositories.DTOs.In;
using NatureExperiments.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebStruct.Shared;

namespace NatureExperiments.Repositories.Repositories
{
    public class NatureExperimentsRepository : INatureExperimentsRepository
    {
        private readonly GenStructContext _context;

        public NatureExperimentsRepository(GenStructContext context)
        {
            _context = context;
        }

        public long CreateNatureExperiment(CreateNatureExperiment input)
        {
            var exp = _context.NatureExperiments.Add(new NatureExperiment
            {
                Name = input.Name,
                Description = input.Description,
                CreatedAt = DateTime.Now,
                CreatedBy = input.CreatedBy,
                NatureExperimentsNatureExperimentsPhysicalAttachments = input.Attachments?
                                                                        .Select
                                                                        (x => new NatureExperimentsNatureExperimentsPhysicalAttachment
                                                                        {
                                                                            File = new NatureExperimentsPhysicalAttachment
                                                                            {
                                                                                FileNameWithoutDotAndExtension = x.FileNameWithoutDotAndExtension,
                                                                                Extension = x.Extension,
                                                                                FileContent = x.FileContent
                                                                            }
                                                                        })
                                                                        ?.ToList()
            });

            _context.SaveChanges();

            return exp.Entity.Id;
        }

        public async Task<long> UpdateNatureExperimentInfoAsync(long experimentId, UpdateNatureExperiment input)
        {
            var experiment = await _context.NatureExperiments.Where(x => x.Id == experimentId).SingleOrDefaultAsync();

            if (experiment == null)
                throw new UsefulException(HttpStatusCode.BadRequest, $"Натурного эксперимента {experimentId} не существует");

            experiment.Name = input.Name;
            experiment.Description = input.Description;
            experiment.LastUpdatedAt = DateTime.Now;
            experiment.LastUpdatedBy = input.LastUpdatedBy;

            _context.SaveChanges();

            return experimentId;
        }
    }
}
