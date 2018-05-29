# TO CREATE THE UI USE
# pyside-uic MainGUI.ui > MainGUI.py

import sys
import os
from PySide.QtGui import *
from PySide.QtCore import *
from MainGUI import Ui_BB_Heatmap_Viewer
from copy import deepcopy


class HeatmapArea:
    def __init__(self):
        self.name = None
        self.time = None
        self.entries = None
        self.stuns = None
        self.throws = None
        self.punches = None
        self.scores = None
        self.dashes = None
        self.x_pos = None
        self.y_pos = None
        self.x_size = None
        self.y_size = None


class MainWindow(QMainWindow, Ui_BB_Heatmap_Viewer):
    def __init__(self):
        super(MainWindow, self).__init__()
        self.setWindowIcon(QIcon('./logo.png'))
        self.setupUi(self)

        # Define Variables
        self.Heatmap_Path = None
        self.Background_Path = None
        self.HeatmapAreaList = list()
        self.pixmap = QPixmap(self.Draw_Area.width(), self.Draw_Area.height())
        self.painter = QPainter(self.pixmap)
        self.colors = {
            "time_box": QColor("#FF0000"),  # RED
            "entries_box": QColor("#FF00FF"),  # PINK
            "throws_box": QColor("#0000FF"),  # BLUE
            "stuns_box": QColor("#00FFFF"),  # LIGHT BLUE
            "dashes_box": QColor("#00FF00"),  # GREEN
            "punches_box": QColor("#FFFF00"),  # YELLOW
            "scores_box": QColor("#AA00FF"),  # PURPLE
            "name_text": QColor("#000000"),  # BLACK
            "time_text": QColor("#000000"),  # BLACK
            "entries_text": QColor("#000000"),  # BLACK
            "throws_text": QColor("#FFFFFF"),  # WHITE
            "stuns_text": QColor("#000000"),  # BLACK
            "dashes_text": QColor("#000000"),  # BLACK
            "punches_text": QColor("#000000"),  # BLACK
            "scores_text": QColor("#000000")  #BLACK
        }

        # Define other widgets
        self.HeatmapSelector = QFileDialog()
        self.HeatmapSelector.setFileMode(QFileDialog.ExistingFile)

        # link widgets
        self.assign_widget_effects()

        # show screen
        self.show()

    def assign_widget_effects(self):
        self.SelectHeatmapFileButton.clicked.connect(self.select_heatmap)
        self.SelectHeatmapBackgroundButton.clicked.connect(self.select_background)
        self.Time_Radio.toggled.connect(self.time_radio_toggle)
        self.Entries_Radio.toggled.connect(self.entries_radio_toggle)
        self.Stuns_Radio.toggled.connect(self.stuns_radio_toggle)
        self.Throws_Radio.toggled.connect(self.throws_radio_toggle)
        self.Dashes_Radio.toggled.connect(self.dashes_radio_toggle)
        self.Punches_Radio.toggled.connect(self.punches_radio_toggle)
        self.Scores_Radio.toggled.connect(self.scores_radio_toggle)

    def count_horizontal(self):
        seen = list()
        count = 0
        for box in self.HeatmapAreaList:
            if box.pos_x not in seen:
                count += 1
                seen.append(box.pos_x)
        return count

    def count_vertical(self):
        seen = list()
        count = 0
        for box in self.HeatmapAreaList:
            if box.pos_y not in seen:
                count += 1
                seen.append(box.pos_y)

    def bounds_horiz(self):
        largest_scale = 0
        min = 0
        max = 0
        for box in self.HeatmapAreaList:
            if box.x_pos < min:
                min = box.x_pos
            if box.x_pos > max:
                max = box.x_pos
            if box.x_size > largest_scale:
                largest_scale = box.x_size
        return min - (largest_scale / 2), max + (largest_scale / 2)

    def bounds_vert(self):
        largest_scale = 0
        min = 0
        max = 0
        for box in self.HeatmapAreaList:
            if box.y_pos < min:
                min = box.y_pos
            if box.y_pos > max:
                max = box.y_pos
            if box.y_size > largest_scale:
                largest_scale = box.y_size
        return min - (largest_scale / 2), max + (largest_scale / 2)

    def draw_heatmap(self):
        if self.Background_Path is not None:
            self.painter = None
            self.pixmap = QPixmap.fromImage(QImage(self.Background_Path))
            self.painter = QPainter(self.pixmap)
        else:
            self.pixmap.fill("#FFFFFF")
        # Set pen color
        text_color = None
        box_color = None
        if self.Time_Radio.isChecked():
            text_color = self.colors["time_text"]
            box_color = self.colors["time_box"]
        elif self.Entries_Radio.isChecked():
            text_color = self.colors["entries_text"]
            box_color = self.colors["entries_box"]
        elif self.Stuns_Radio.isChecked():
            text_color = self.colors["stuns_text"]
            box_color = self.colors["stuns_box"]
        elif self.Throws_Radio.isChecked():
            text_color = self.colors["throws_text"]
            box_color = self.colors["throws_box"]
        elif self.Dashes_Radio.isChecked():
            text_color = self.colors["dashes_text"]
            box_color = self.colors["dashes_box"]
        elif self.Punches_Radio.isChecked():
            text_color = self.colors["punches_text"]
            box_color = self.colors["punches_box"]
        elif self.Scores_Radio.isChecked():
            text_color = self.colors["scores_text"]
            box_color = self.colors["scores_box"]

        # determine geometry of the draw area
        width = self.pixmap.width()
        height = self.pixmap.height()
        center_x = width / 2
        center_y = height / 2

        # determine how many boxes are needed horizontally
        min_h, max_h = self.bounds_horiz()
        total_diff_h = max_h - min_h
        # determine how many boxes are needed vertically
        min_v, max_v = self.bounds_vert()
        total_diff_v = max_v - min_v

        # determine the size of the boxes
        if total_diff_h >= total_diff_v:
            unit_size = width / total_diff_h
        else:
            unit_size = height / total_diff_v

        # Draw the boxes
        for box in self.HeatmapAreaList:
            # Get values for this box
            text = "ERR"
            max_val = 0
            box_val = 0
            alpha = 0
            if self.Time_Radio.isChecked():
                text = "{0:0.1f}".format(box.time)
                max_val = max([a.time for a in self.HeatmapAreaList])
                box_val = box.time
                if box_val == 0:
                    alpha = 0
                else:
                    alpha = 64 + ((box_val / max_val) * (255 - 64))   # Get the alpha value of the box (up to 255)
            elif self.Entries_Radio.isChecked():
                text = str(box.entries)
                max_val = max([a.entries for a in self.HeatmapAreaList])
                box_val = box.entries
                if box_val == 0:
                    alpha = 0
                else:
                    alpha = 64 + ((box_val / max_val) * (255 - 64))   # Get the alpha value of the box (up to 255)
            elif self.Stuns_Radio.isChecked():
                text = str(box.stuns)
                max_val = max([a.stuns for a in self.HeatmapAreaList])
                box_val = box.stuns
                if box_val == 0:
                    alpha = 0
                else:
                    alpha = 64 + ((box_val / max_val) * (255 - 64))   # Get the alpha value of the box (up to 255)
            elif self.Throws_Radio.isChecked():
                text = str(box.throws)
                max_val = max([a.throws for a in self.HeatmapAreaList])
                box_val = box.throws
                if box_val == 0:
                    alpha = 0
                else:
                    alpha = 64 + ((box_val / max_val) * (255 - 64))   # Get the alpha value of the box (up to 255)
            elif self.Dashes_Radio.isChecked():
                text = str(box.dashes)
                max_val = max([a.dashes for a in self.HeatmapAreaList])
                box_val = box.dashes
                if box_val == 0:
                    alpha = 0
                else:
                    alpha = 64 + ((box_val / max_val) * (255 - 64))   # Get the alpha value of the box (up to 255)
            elif self.Punches_Radio.isChecked():
                text = str(box.punches)
                max_val = max([a.punches for a in self.HeatmapAreaList])
                box_val = box.punches
                if box_val == 0:
                    alpha = 0
                else:
                    alpha = 64 + ((box_val / max_val) * (255 - 64))   # Get the alpha value of the box (up to 255)
            elif self.Scores_Radio.isChecked():
                text = str(box.scores)
                max_val = max([a.scores for a in self.HeatmapAreaList])
                box_val = box.scores
                if box_val == 0:
                    alpha = 0
                else:
                    alpha = 64 + ((box_val / max_val) * (255 - 64))   # Get the alpha value of the box (up to 255)
            # Draw the rectangle
            self.painter.setPen(QPen("black"))
            # drawRect(x, y, width, height) position in the top left corner
            # coordinate is center of area, offset by position of box, offset by half size in right direction
            cent_x = center_x + (box.x_pos * unit_size)
            cent_y = center_y + ((box.y_pos * -1) * unit_size)
            width = (box.x_size * unit_size)
            height = (box.y_size * unit_size)
            x_min = cent_x - ((box.x_size / 2) * unit_size)
            y_min = cent_y - ((box.y_size / 2) * unit_size)
            x_max = x_min + width
            y_max = y_min + height
            path = QPainterPath()
            path.addRect(x_min, y_min, width, height)
            box_color.setAlpha(int(alpha))
            self.painter.fillPath(path, box_color)
            self.painter.drawPath(path)
            # Draw the Text
            self.painter.setPen(QPen(text_color))
            self.painter.drawText(cent_x, cent_y, text)
            self.Draw_Area.setPixmap(self.pixmap)

    def read_heatmap(self):
        """
        Read the contents of the selected heatmap file and generate an image from it.
        :return: None
        """
        lines = None
        valid = False
        if (self.Heatmap_Path is not None) and (os.path.exists(self.Heatmap_Path)):
            with open(self.Heatmap_Path) as hmf:
                lines = [x.strip() for x in hmf.readlines()]
            # lines[0] is identifier
            if lines[0] == "BARGAIN BASH HEATMAP":
                valid = True
            if valid:
                self.HeatmapAreaList.clear()
                # lines[1] is level name
                level_line = lines[1]
                self.HeatmapSceneNameLabel.setText(level_line.split(",", 1)[1])
                self.HeatmapPathLabel.setText(self.Heatmap_Path.rsplit("/", 1)[1])
                if self.Background_Path is not None:
                    self.CurrentBackgroundPathLabel.setText(self.Background_Path.rsplit("/", 1)[1])
                # lines[2] is header so ignore it
                # lines[3:] is all data
                for line in lines[3:]:
                    split_line = line.split(",")
                    temp_area = HeatmapArea()
                    temp_area.name = split_line[0]
                    temp_area.time = float(split_line[1])
                    temp_area.entries = int(split_line[2])
                    temp_area.throws = int(split_line[3])
                    temp_area.stuns = int(split_line[4])
                    temp_area.dashes = int(split_line[5])
                    temp_area.punches = int(split_line[6])
                    temp_area.scores = int(split_line[7])
                    temp_area.x_pos = float(split_line[8])
                    temp_area.y_pos = float(split_line[9])
                    temp_area.x_size = float(split_line[10])
                    temp_area.y_size = float(split_line[11])
                    self.HeatmapAreaList.append(deepcopy(temp_area))
        self.draw_heatmap()

    def select_heatmap(self):
        selected = self.HeatmapSelector.getOpenFileName(self, "Open Heatmap", os.getcwd(), "CSV Files (*.csv)")[0]
        if selected is not None:
            self.Heatmap_Path = selected
            self.read_heatmap()

    def select_background(self):
        selected = self.HeatmapSelector.getOpenFileName(self, "Open Background Image", os.getcwd(), "PNG Files (*.png)")[0]
        if selected is not None:
            self.Background_Path = selected
            if self.Heatmap_Path is not None:
                self.read_heatmap()

    def time_radio_toggle(self):
        self.draw_heatmap()

    def entries_radio_toggle(self):
        self.draw_heatmap()

    def stuns_radio_toggle(self):
        self.draw_heatmap()

    def throws_radio_toggle(self):
        self.draw_heatmap()

    def dashes_radio_toggle(self):
        self.draw_heatmap()

    def punches_radio_toggle(self):
        self.draw_heatmap()

    def scores_radio_toggle(self):
        self.draw_heatmap()

if __name__ == '__main__':
    app = QApplication(sys.argv)
    mainWin = MainWindow()
    ret = app.exec_()
    sys.exit(ret)
