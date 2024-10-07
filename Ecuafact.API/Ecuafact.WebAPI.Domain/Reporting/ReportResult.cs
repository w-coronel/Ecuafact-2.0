﻿using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Reporting
{
    public class ReportResult
    {
        public ReportResult(byte[] content)
        {
            Content = content;
        }

        public byte[] Content { get; set; }
        public string MimeType { get; set; }
        public string Encoding { get; set; }
        public string Extension { get; set; }
        public string[] Streams { get; set; }
        public Warning[] Warnings { get; set; }

    }
}