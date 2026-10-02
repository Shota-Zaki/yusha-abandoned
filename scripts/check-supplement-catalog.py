#!/usr/bin/env python3
"""Read-only YA-D03 source/current reference consistency; no runtime acceptance."""
from pathlib import Path
import html
import re
ROOT = Path(__file__).resolve().parents[1]
def text(value):
    return html.unescape(re.sub(r'<[^>]+>', '', value)).strip()
def md_rows(path):
    return [[x.strip() for x in line.strip('|').split('|')]
            for line in (ROOT / path).read_text().splitlines() if line.startswith('|')]
source = (ROOT / 'docs/archive/design-supplement-v0.1.1.html').read_text()
section = source.split('<section id="section-1">')[1].split('</section>')[0]
tables = re.findall(r'<table>(.*?)</table>', section, re.S)
rows = [[text(c) for c in re.findall(r'<td>(.*?)</td>', row, re.S)]
        for row in re.findall(r'<tr>(.*?)</tr>', tables[0], re.S)]
rows = [row for row in rows if row]
jobs = {row[0]: row for row in md_rows('docs/design/JOBS.md') if re.fullmatch(r'J\d{2}', row[0])}
skills = {row[0]: row for row in md_rows('docs/design/SKILLS.md') if re.fullmatch(r'J\d{2}', row[0])}
assert len(rows) == len(jobs) == len(skills) == 50
for row in rows:
    key, name, tier, prerequisites, programs = row
    assert jobs[key][1] == name, (key, 'name')
    assert tier in {'basic', 'advanced', 'composite'}, (key, 'tier')
    expected_tier = 'basic' if int(key[1:]) <= 20 else 'advanced' if int(key[1:]) <= 40 else 'composite'
    assert tier == expected_tier, (key, 'tier boundary')
    if expected_tier != 'basic':
        name_ids = {value[1]: ident for ident, value in jobs.items()}
        prerequisite_ids = '+'.join(name_ids[name.strip()] for name in prerequisites.split('+'))
        assert jobs[key][2] == prerequisite_ids, (key, 'prerequisites', jobs[key][2], prerequisite_ids)
    actual = [cell.split(':', 1)[1].strip() for cell in skills[key][1:]]
    expected = [part.strip() for part in re.split(r'[,、/／]', programs)]
    assert len(actual) == 6, (key, 'skill count')
    assert actual == expected, (key, 'programs', actual, expected)
spell_rows = [[text(c) for c in re.findall(r'<td>(.*?)</td>', row, re.S)]
              for row in re.findall(r'<tr>(.*?)</tr>', tables[1], re.S)]
spell_rows = [row for row in spell_rows if row]
current_spell = {row[0]: row for row in md_rows('docs/design/SKILLS.md') if row[0] in {'火','風','水','雷','氷','土','光','闇'}}
assert len(spell_rows) == len(current_spell) == 8
for row in spell_rows:
    assert row[1:] == current_spell[row[0]][2:6], (row[0], 'stage names')
print('PASS: 50 job names/tiers/prerequisites, 300 fixed program references, 32 spell stage names; static source/current agreement only')
