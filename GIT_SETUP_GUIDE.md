# Git Setup Guide - FallingSand Unity v1.0

## 🎯 Quick Setup (Automated)

**Just run this:**

```bash
.\git-setup.bat
```

This will:
1. ✅ Clean up legacy Java/Gradle folders
2. ✅ Initialize Git repository  
3. ✅ Configure Git settings
4. ✅ Stage all files
5. ✅ Show you what's ready to commit

---

## 📋 Manual Setup (Step-by-Step)

If you prefer to do it manually:

### Step 1: Clean Up Legacy Files

Delete these folders manually:
- `core/` - Old Java/libGDX assets
- `.gradle/` - Gradle build cache

```bash
# PowerShell
Remove-Item -Path "core" -Recurse -Force
Remove-Item -Path ".gradle" -Recurse -Force
```

### Step 2: Initialize Git

```bash
git init
```

### Step 3: Configure Git (First Time Only)

```bash
git config user.name "Johnny-Z13"
git config user.email "your-email@example.com"  # Update this!
```

### Step 4: Stage All Files

```bash
git add .
```

### Step 5: Review What's Being Committed

```bash
git status
```

**Expected files to commit:**
- ✅ `Assets/` - Your Unity project files
- ✅ `ProjectSettings/` - Unity configuration
- ✅ `Packages/` - Package manifest
- ✅ All `.md` documentation files
- ✅ `media/` - Demo GIFs
- ✅ `.gitignore` - Git ignore rules

**Should NOT see (gitignored):**
- ❌ `Library/` - Unity cache
- ❌ `Temp/` - Temporary files
- ❌ `Logs/` - Log files
- ❌ `UserSettings/` - User-specific settings
- ❌ `*.csproj`, `*.sln` - Auto-generated

### Step 6: Create Initial Commit

Use the pre-written commit message:

```bash
git commit -F COMMIT_MESSAGE.txt
```

Or write your own:

```bash
git commit -m "Initial commit: Unity 6 Falling Sand v1.0

- Complete port from Java/libGDX to Unity 6
- 30+ element types with cellular automata
- Comprehensive documentation
- Credits: Original Java tutorial by CodeNMore"
```

### Step 7: Add Remote Repository

```bash
git remote add origin https://github.com/Johnny-Z13/FallingSand.git
```

### Step 8: Push to GitHub

```bash
git branch -M main
git push -u origin main
```

### Step 9: Create Release Tag (Optional)

```bash
git tag -a v1.0 -m "Version 1.0: Initial Unity release"
git push origin v1.0
```

---

## ✅ Pre-Commit Checklist

Before pushing, verify:

- [ ] Legacy folders deleted (`core/`, `.gradle/`)
- [ ] `.gitignore` is the Unity version (not Java)
- [ ] Unity editor closed (avoid file locks)
- [ ] `Library/` folder NOT in commit
- [ ] All documentation files included
- [ ] Commit message is descriptive
- [ ] Remote URL is correct

---

## 🚨 Troubleshooting

### "Failed to remove 'core' - directory not empty"

Close Unity and any file explorers, then try:

```bash
# PowerShell (Run as Administrator)
Get-ChildItem -Path "core" -Recurse | Remove-Item -Force -Recurse
Remove-Item -Path "core" -Force
```

### "Permission denied" errors

- Close Unity Editor
- Close any open terminals in that directory
- Run command prompt/PowerShell as Administrator

### Files won't stage / Git ignoring everything

Check your `.gitignore` - it should be the Unity version I created, not the old Java one.

### Huge commit size (100s of MB)

If `Library/` is being included:
```bash
git rm -r --cached Library/
git commit -m "Remove Library folder from git"
```

---

## 📊 Expected Repository Size

**Initial commit should be:**
- ~50-100 files
- ~5-10 MB total
- Mostly `.cs` scripts, `.md` docs, and `.png` textures

**If it's much larger:**
- Check that `Library/` is gitignored
- Remove any large binary files

---

## 🎉 After First Push

Your GitHub repo will show:

```
FallingSand/
├── Assets/           (Unity project)
├── ProjectSettings/  (Unity config)
├── Packages/         (Package manifest)
├── media/            (Demo GIFs)
├── .gitignore
├── README.md
├── ELEMENTS_REFERENCE.md
├── SYSTEM_SETTINGS.md
├── RENDER_PIPELINE.md
├── PHYSICS_SYSTEM.md
├── ARCHITECTURE.md
├── QUICK_START.md
└── SETUP_INSTRUCTIONS.md
```

---

## 📝 Future Commits

For future changes:

```bash
# Stage changes
git add .

# Commit with message
git commit -m "feat(elements): add Ice element with freezing behavior"

# Push
git push
```

**Commit message format:**
- `feat(scope): description` - New feature
- `fix(scope): description` - Bug fix
- `docs(scope): description` - Documentation
- `perf(scope): description` - Performance improvement
- `refactor(scope): description` - Code refactoring

---

## 🔗 Useful Links

- **Your Repo**: https://github.com/Johnny-Z13/FallingSand
- **Original Java Tutorial**: https://youtu.be/5Ka3tbbT-9E
- **Unity Docs**: https://docs.unity3d.com/

---

**Ready to share your project with the world! 🚀**
