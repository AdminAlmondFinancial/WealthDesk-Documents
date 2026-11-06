@echo off
cd /d "%~dp0"
if not exist node_modules call npm install
if not exist .env.local copy .env.example .env.local >nul
call npm run dev
