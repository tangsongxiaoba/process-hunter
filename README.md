<h1 align="center">ProcessHunter</h1>

<h4 align="center">Kill specific process by file hash.</h4>

<p align="center">
    <img alt="GitHub code size in bytes" src="https://img.shields.io/github/languages/code-size/tangsongxiaoba/ProcessHunter?color=12bf00&style=flat-square">
    <img alt="GitHub release (latest by date including pre-releases)" src="https://img.shields.io/github/v/release/tangsongxiaoba/ProcessHunter?include_prereleases&style=flat-square">
    <img alt="GitHub Repo stars" src="https://img.shields.io/github/stars/tangsongxiaoba/processHunter?color=F6001F&style=flat-square">
    <img alt="GitHub all releases" src="https://img.shields.io/github/downloads/tangsongxiaoba/ProcessHunter/total?style=flat-square">
    <img alt="GitHub" src="https://img.shields.io/github/license/tangsongxiaoba/ProcessHunter?style=flat-square">
</p>

<p align="center">
  <a href="#background">Background</a> •
  <a href="#features">Features</a> •
  <a href="#requirements">Requirements</a> •
  <a href="#usage">Usage</a> •
  <a href="#download">Download</a> •
  <a href="#license">License</a>
</p>

## Background
see [Background.md](Background.md)

## Features

* Kill specific process by hash. 
  - Use [uranium62/xxHash](https://github.com/uranium62/xxHash)
* Click-to-run or run as a Windows background service
  - Use [Worker Services](https://docs.microsoft.com/en-us/dotnet/core/extensions/workers)

## Requirements

Windows x64

## Usage

Recommend making it to run as a service.
```
sc.exe create ProcessHunter binPath=programFullPath // to create a service
sc.exe start ProcessHunter // to start a service
sc.exe stop ProcessHunter // to stop a service
sc.exe delete ProcessHunter // to delete a service
```

## License
GNU Affero General Public License 3.0
