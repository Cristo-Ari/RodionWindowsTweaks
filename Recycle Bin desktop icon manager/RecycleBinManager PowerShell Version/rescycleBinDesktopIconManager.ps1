param (
    [ValidateSet("show", "hide", "status")]
    [string]$Action
)

function refresh-desktop {
    Add-Type (
    @"
using System;
using System.Runtime.InteropServices;
public class RefreshDesktop {
    [DllImport("shell32.dll")] public static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);
}
"@
    )
    [RefreshDesktop]::SHChangeNotify(0x8000000, 0x1000, [IntPtr]::Zero, [IntPtr]::Zero)
}

function hide-recycle-bin {
    reg add "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel" /v "{645FF040-5081-101B-9F08-00AA002F954E}" /t REG_DWORD /d 1 /f
    refresh-desktop
}

function show-recycle-bin {
    reg add "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel" /v "{645FF040-5081-101B-9F08-00AA002F954E}" /t REG_DWORD /d 0 /f
    refresh-desktop
}

function isRecycleBinShowing {
    $regPath = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel"
    $recycleBinKey = "{645FF040-5081-101B-9F08-00AA002F954E}"
    $value = Get-ItemProperty -Path $regPath -Name $recycleBinKey -ErrorAction Stop
    if ($value.$recycleBinKey -eq 1) {
        return $false
    }
    elseif ($value.$recycleBinKey -eq 0) {
        return $true
    }
    else {
        return $false
    }
}

function print-recycle-bin-state {
    if (isRecycleBinShowing) {
        Write-Output "Сейчас отображена иконка корзины"
    } else {
        Write-Output "Сейчас скрыта иконка корзины"
    }
}

function runRecycleBinManager {
    do {
        print-recycle-bin-state
        switch (Read-Host "1- Показать корзину  2- Скрыть корзину  3- Проверить состояние  0- Выход") {
            1 {
                show-recycle-bin
            }
            2 {
                hide-recycle-bin
            }
            3 {
                print-recycle-bin-state
            }
            0 {
                return
            }
            default {
                return
            }
        }
    } while ($true)
}

if ($Action -eq "show") {
    show-recycle-bin
} elseif ($Action -eq "hide") {
    hide-recycle-bin
} elseif ($Action -eq "status") {
    print-recycle-bin-state
} else {
    runRecycleBinManager
}