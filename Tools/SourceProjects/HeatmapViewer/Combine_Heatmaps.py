import os
import time
from copy import deepcopy

class heatmap_block:
    def __init__(self):
        self.name = ""
        self.time = 0
        self.entries = 0
        self.throws = 0
        self.stuns = 0
        self.dashs = 0
        self.punches = 0
        self.scores = 0
        self.position_x = 0
        self.position_y = 0
        self.size_x = 0
        self.size_y = 0
        self.ground_slams = 0
        self.level = ""

    def isAlike(self, other):
        if (other.position_x == self.position_x) and \
                (other.position_y == self.position_y) and \
                (other.size_x == self.size_x) and \
                (other.size_y == self.size_y) and \
                (self.level == other.level) and \
                (other.name == self.name):
            return True
        return False

    def combine(self, other):
        self.time = self.time + other.time
        self.entries = self.entries + other.entries
        self.throws = self.throws + other.throws
        self.stuns = self.stuns + other.stuns
        self.dashs = self.dashs + other.dashs
        self.punches = self.punches + other.punches
        self.scores = self.scores + other.scores
        self.ground_slams = other.ground_slams


def combine():
    dir = "..\\..\\..\\StatLogs\\Heatmaps\\"
    assert os.path.exists(dir)
    heatblocks = []
    header = ""
    for file in os.listdir(dir):
        if os.path.isfile(os.path.join(dir, file)) and file.endswith(".CSV") and file.startswith("Heatmap"):
            with open(os.path.join(dir, file), 'r') as in_f:
                lines = in_f.readlines()
                if "BARGAIN BASH HEATMAP" in lines[0]:
                    if header == "":
                        header = lines[2]
                    level = lines[1].strip().split(",")[1]
                    for line in lines[3:]:
                        new_block = heatmap_block()
                        new_block.level = level
                        sep_line = line.split(",")
                        new_block.name = sep_line[0]
                        new_block.time = float(sep_line[1])
                        new_block.entries = int(sep_line[2])
                        new_block.throws = int(sep_line[3])
                        new_block.stuns = int(sep_line[4])
                        new_block.dashs = int(sep_line[5])
                        new_block.punches = int(sep_line[6])
                        new_block.scores = int(sep_line[7])
                        new_block.position_x = float(sep_line[8])
                        new_block.position_y = float(sep_line[9])
                        new_block.size_x = float(sep_line[10])
                        new_block.size_y = float(sep_line[11])
                        new_block.ground_slams = int(sep_line[12])
                        match = False
                        for block in heatblocks:
                            if block.isAlike(new_block):
                                block.combine(new_block)
                                match = True
                        if not match:
                            heatblocks.append(deepcopy(new_block))
                in_f.close()
    # get list of levels
    levels = []
    for block in heatblocks:
        if block.level not in levels:
            levels.append(block.level)
    # write a file for each level
    for level in levels:
        with open(os.path.join(dir, "Combined_{lvl}_{time}.CSV".format(lvl=level, time=time.strftime('%Y%m%d'))), 'w+') as out:
            out.write("BARGAIN BASH HEATMAP\n")
            out.write("Level,{lvl}\n".format(lvl=level))
            out.write(header)
            for block in heatblocks:
                if block.level == level:
                    out.write("{n},{ti},{e},{th},{st},{d},{p},{sc},{px},{py},{six},{siy},{gs}\n".format(
                        n=block.name,
                        ti=block.time,
                        e=block.entries,
                        th=block.throws,
                        st=block.stuns,
                        d=block.dashs,
                        p=block.punches,
                        sc=block.scores,
                        px=block.position_x,
                        py=block.position_y,
                        six=block.size_x,
                        siy=block.size_y,
                        gs=block.ground_slams
                    ))
            out.close()

if __name__ == "__main__":
    combine()