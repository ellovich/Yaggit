$sourceFolder = "C:\Users\ello\.nuget\packages"
$destinationFolder = "C:\Users\ello\.nuget\onlyNugets"

# Устанавливаем поведение при ошибках: останавливаем выполнение при первой ошибке
$ErrorActionPreference = "Stop"

try {
    # Проверяем существование исходной папки
    if (!(Test-Path -Path $sourceFolder)) {
        throw "Исходная папка не найдена: $sourceFolder"
    }

    # Если целевая папка не существует, создаём её; если существует, очищаем
    if (!(Test-Path -Path $destinationFolder)) {
        New-Item -Path $destinationFolder -ItemType Directory | Out-Null
        Write-Host "Создана целевая папка: $destinationFolder"
    } else {
        Write-Host "Очищаем целевую папку: $destinationFolder"
        Get-ChildItem -Path $destinationFolder -Recurse | Remove-Item -Force -Recurse
    }

    # Получаем файлы с расширением .nupkg из исходной папки
    $nupkgFiles = Get-ChildItem -Path $sourceFolder -Filter *.nupkg -Recurse 

    # Проверяем, найдены ли файлы
    if ($nupkgFiles.Count -eq 0) {
        Write-Host "Файлы с расширением .nupkg не найдены в папке $sourceFolder"
    } else {
        Write-Host "Копирование файлов..."
        foreach ($file in $nupkgFiles) {
            $destinationPath = Join-Path -Path $destinationFolder -ChildPath $file.Name
            Copy-Item -Path $file.FullName -Destination $destinationPath
            Write-Host "Скопирован: $($file.FullName) -> $destinationPath"
        }
        Write-Host "Копирование завершено."
    }
}
catch {
    Write-Host "Произошла ошибка: $_"
}
finally {
    # Ожидаем нажатия клавиши, чтобы окно не закрывалось автоматически
    Read-Host -Prompt "Нажмите Enter для выхода"
}