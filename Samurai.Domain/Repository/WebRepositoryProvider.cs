﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Repository
{
  public interface IWebRepositoryProvider
  {
    WebRepository CreateWebRepository(DateTime repositoryDate);
  }

  public class WebRepositoryProvider : IWebRepositoryProvider
  {
    private readonly string repositoryType;
    private readonly string basePath;

    public WebRepositoryProvider(string repositoryType, string basePath)
    {
      if (string.IsNullOrEmpty(repositoryType)) throw new ArgumentNullException("repositoryType");
      if (string.IsNullOrEmpty(basePath)) throw new ArgumentNullException("folderName");
      this.repositoryType = repositoryType;
      this.basePath = basePath;
    }

    public WebRepository CreateWebRepository(DateTime repositoryDate)
    {
      if (this.repositoryType == "SaveTestData")
        return new WebRepositorySaveTestData(repositoryDate.ToShortDateString().Replace("/", "-"));
      else if (this.repositoryType == "TestData")
        return new WebRepositoryTestData(repositoryDate.ToShortDateString().Replace("/", "-"));
      else
        return new WebRepository(basePath);
    }
  }
}