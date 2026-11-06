@echo off
cd /d "%~dp0AI.Client.Web"
if not exist node_modules call npm install
if not exist .env.local copy .env.example .env.local >nul
call npm run dev
