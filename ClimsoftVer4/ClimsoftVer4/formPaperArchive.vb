﻿Public Class formPaperArchive
    Dim dbconn As New MySql.Data.MySqlClient.MySqlConnection
    Dim dbConnectionString As String
    Dim da As MySql.Data.MySqlClient.MySqlDataAdapter
    Dim ds As New DataSet
    Dim sql As String
    Dim rec As Integer
    Dim Kount As Integer
    Dim FileNm As String


    Private Sub cmdcloses_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub cmdClose_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()

    End Sub

    'Private Sub cmdFolder_Click(sender As Object, e As EventArgs)
    '    Dim fld As String
    '    folderPaperArchive.ShowDialog()
    '    txtSelectedFolder.Text = folderPaperArchive.SelectedPath
    '    fld = txtSelectedFolder.Text
    '    ListFiles(fld)
    'End Sub
    Sub ListFiles(flds As String)
        ' Declare and assign folder variables
        Dim f1 As New IO.DirectoryInfo(flds)
        Dim d1 As IO.FileInfo() = f1.GetFiles()
        Dim da1 As IO.FileInfo
        Dim fls As String

        lstvFiles.Columns.Add("Files", 500, HorizontalAlignment.Left)
        'list the names of all files in the specified directory
        For Each da1 In d1
            fls = da1.Name
            'listImages.Items.Add(da1)
            lstvFiles.Items.Add(fls)
        Next

        FilesListSatus(True)
        'chkFiles.Text = "Unselect All"
        'To filter search change f1.GetFiles() to di.GetFiles(“.extionsion”)
    End Sub


    Private Sub cmdFolder_Click_1(sender As Object, e As EventArgs) Handles cmdFolder.Click
        Dim fld As String
        folderPaperArchive.ShowDialog()
        txtSelectedFolder.Text = folderPaperArchive.SelectedPath
        fld = txtSelectedFolder.Text

        lstvFiles.Clear()
        ListFiles(fld)
    End Sub

    Private Sub chkFiles_CheckedChanged(sender As Object, e As EventArgs) Handles chkFiles.CheckedChanged
        If chkFiles.CheckState = False Then
            FilesListSatus(False)
            chkFiles.Text = "Select All"
            'cmdArchive.Enabled = False
        Else
            chkFiles.Visible = True
            FilesListSatus(True)
            chkFiles.Text = "UnSelect All"
            cmdArchive.Enabled = True
        End If
    End Sub
    Sub FilesListSatus(sts As Boolean)
        If lstvFiles.Items.Count > 0 Then
            cmdArchive.Enabled = True
            chkFiles.Visible = True
        Else
            cmdArchive.Enabled = False
            chkFiles.Visible = False
            Exit Sub
        End If

        For i = 0 To lstvFiles.Items.Count - 1
            lstvFiles.Items(i).Checked = sts
        Next
    End Sub

    Private Sub cmdArchive_Click(sender As Object, e As EventArgs) Handles cmdArchive.Click
        Dim imgFile As String
        On Error GoTo err
        'UpdateArchive(txtSelectedFolder.Text, lstvFiles.Items(0).Text)

        For i = 0 To lstvFiles.Items.Count - 1
            imgFile = txtSelectedFolder.Text & "\" & lstvFiles.Items(i).Text
            If lstvFiles.Items(i).Checked Then
                IO.File.Copy(imgFile, "c:\images\" & lstvFiles.Items(i).Text, True)
                UpdateArchive(txtSelectedFolder.Text, lstvFiles.Items(i).Text)
            End If
        Next
        Exit Sub
err:
        MsgBox(Err.Description)
    End Sub
    Sub UpdateArchive(FilePth As String, FileNm As String)
        Dim siz, count, i As Integer
        Dim imgfile, str, stn, frm, yy, mm, dd, hh, dt, datetim As String

        imgfile = FilePth & "\" & FileNm
       
        siz = Len(FileNm)
        count = 0
        str = ""
        stn = ""
        frm = ""
        datetim = ""
        i = 1
        For i = 1 To siz
            str = str & Mid(FileNm, i, 1)
            If Mid(FileNm, i, 1) = "-" Then
                count = count + 1
                'Construct StationId string
                If count = 1 Then
                    stn = Mid(str, 1, Len(str) - 1)
                    'MsgBox(stn)
                    str = ""
                End If
                'Construct FormCode string
                If count = 2 Then
                    frm = Mid(str, 1, Len(str) - 1)
                    'MsgBox(frm)
                    str = ""
                End If
            End If
            'Construct datetime string
            If Mid(FileNm, i, 1) = "." Then
                dt = Mid(str, 1, Len(str) - 1)
                'MsgBox(dt)
                yy = Mid(dt, 1, 4)
                mm = Mid(dt, 5, 2)
                dd = Mid(dt, 7, 2)
                hh = Mid(dt, 9, 2)
                datetim = yy & "/" & mm & "/" & dd & " " & hh & ":00:00"
                'MsgBox(datetim)
                If IsDate(datetim) Then
                    ArchiveRecord(stn, frm, yy, mm, dd, hh, imgfile)
                Else
                    MsgBox("Incorrect Datetime Structure")
                End If
                Exit For
            End If
        Next

    End Sub

    Private Sub MenuPaperArchive_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles MenuPaperArchive.ItemClicked

    End Sub

    Sub ArchiveRecord(stn As String, frm As String, yr As Integer, mn As Integer, dy As Integer, hr As Integer, img As String)
        On Error GoTo Err
        'On Error Resume Next
        Dim cb As New MySql.Data.MySqlClient.MySqlCommandBuilder(da)
        Dim dsNewRow As DataRow

        'MsgBox(stn)
        'Dim recCommit As New dataEntryGlobalRoutines

        dsNewRow = ds.Tables("paperarchive").NewRow

        dsNewRow.Item("belongsTo") = stn
        dsNewRow.Item("code") = frm
        dsNewRow.Item("description") = frm & " From " & stn
        dsNewRow.Item("year") = yr
        dsNewRow.Item("month") = mn
        dsNewRow.Item("day") = dy
        dsNewRow.Item("hour") = hr
        dsNewRow.Item("image") = img

        'Add a new record to the data source table
        ds.Tables("paperarchive").Rows.Add(dsNewRow)
        da.Update(ds, "paperarchive")
        'MsgBox("Record Archived")
        Exit Sub
Err:
        MsgBox(Err.Number & " : " & Err.Description)

    End Sub

    Private Sub formPaperArchive_Load(sender As Object, e As EventArgs) Handles Me.Load

        dbConnectionString = frmLogin.txtusrpwd.Text
        dbconn.ConnectionString = dbConnectionString
        dbconn.Open()

        sql = "SELECT * FROM paperarchive"
        da = New MySql.Data.MySqlClient.MySqlDataAdapter(sql, dbconn)
        da.Fill(ds, "paperarchive")

        FillList(txtStationArchive, "station", "stationId")
        FillList(txtStation, "station", "stationId")
    End Sub

    Private Sub cmdView_Click(sender As Object, e As EventArgs) Handles cmdView.Click
        Dim stn, frm, img As String
        Dim yr, mn, dy, hr, num, mxrows As Integer

        sql = "SELECT * FROM paperarchive"
        da = New MySql.Data.MySqlClient.MySqlDataAdapter(sql, dbconn)
        da.Fill(ds, "paperarchive")

        mxrows = ds.Tables("paperarchive").Rows.Count

        For num = 0 To mxrows
            stn = ds.Tables("paperarchive").Rows(num).Item("belongsTo")
            frm = ds.Tables("paperarchive").Rows(num).Item("code")
            yr = ds.Tables("paperarchive").Rows(num).Item("year")
            mn = ds.Tables("paperarchive").Rows(num).Item("month")
            dy = ds.Tables("paperarchive").Rows(num).Item("day")
            hr = ds.Tables("paperarchive").Rows(num).Item("hour")
            hr = ds.Tables("paperarchive").Rows(num).Item("hour")
            img = ds.Tables("paperarchive").Rows(num).Item("image")
            If stn = txtStation.Text And frm = txtForm.Text And yr = txtYY.Text And mn = txtMM.Text And dy = txtDD.Text And hr = txtHH.Text Then
                ShowImage(img)
                Exit For
            End If
        Next


    End Sub
    Sub ShowImage(img As String)
        System.Diagnostics.Process.Start(img)
    End Sub

    Private Sub cmdImageFile_Click(sender As Object, e As EventArgs) Handles cmdImageFile.Click
        Dim img As String

        OpenFilePaperArchive.ShowDialog()
        img = OpenFilePaperArchive.FileName
        txtImageFile.Text = img
        'dir = IO.Directory.GetParent(txtImageFile.Text).FullName
        'fld = Len(dir)
        'MsgBox(Mid(txtImageFile.Text, fld + 2, Len(txtImageFile.Text) - fld + 1))
        'xt = InStr(txtImageFile.Text, ".")
        'ext = Mid(txtImageFile.Text, xt + 1, Len(txtImageFile.Text) - 1)

        'FileNm = dir & "\" & txtStationArchive.Text & "-" & txtFormId.Text & "-" & Format(Val(txtYear.Text), "00") & Format(Val(txtMonth.Text), "00") _
        '    & Format(Val(txtDay.Text), "00") & Format(Val(txtHour.Text), "00") & "." & ext

        'y = Format(Val(txtYear.Text), "00")
        'm = Format(Val(txtDay.Text), "00")
        'd = Format(Val(txtMonth.Text), "00")
        'h = Format(Val(txtHour.Text), "00")
        'MsgBox(y & m & d & h)
    End Sub


    'Private Sub Button1_Click(sender As Object, e As EventArgs) Handles cmdArchiveUnstructure.Click
    '    MsgBox(Format(Val(txtMonth.Text), "00"))
    '    'MsgBox(Format(txtDay.Text)        
    '    'MsgBox(Format(txtHour.Text, "00"))
    'End Sub

    Private Sub cmdArchiveUnstructure_Click(sender As Object, e As EventArgs) Handles cmdArchiveUnstructure.Click
        Dim dir, ext As String
        Dim fld, xt As Integer
        Dim stn, frm, y, m, d, h As String

        Dir = IO.Directory.GetParent(txtImageFile.Text).FullName
        fld = Len(Dir)
        'MsgBox(Mid(txtImageFile.Text, fld + 2, Len(txtImageFile.Text) - fld + 1))
        xt = InStr(txtImageFile.Text, ".")
        ext = Mid(txtImageFile.Text, xt + 1, Len(txtImageFile.Text) - 1)

        FileNm = txtStationArchive.Text & "-" & txtFormId.Text & "-" & Format(Val(txtYear.Text), "00") & Format(Val(txtMonth.Text), "00") _
            & Format(Val(txtDay.Text), "00") & Format(Val(txtHour.Text), "00") & "." & ext

        stn = txtStationArchive.Text
        frm = txtStationArchive.Text
        y = Format(Val(txtYear.Text), "00")
        m = Format(Val(txtDay.Text), "00")
        d = Format(Val(txtMonth.Text), "00")
        h = Format(Val(txtHour.Text), "00")

        'MsgBox(stn)

        IO.File.Copy(txtImageFile.Text, "c:\images\" & FileNm, True)
        ArchiveRecord(stn, frm, y, m, d, h, "c:\images\" & FileNm)

        'UpdateArchive(txtSelectedFolder.Text, lstvFiles.Items(i).Text)

        'MsgBox(Format(txtDay.Text)        
        'MsgBox(Format(txtHour.Text, "00"))
    End Sub
    Sub FillList(ByRef lst As ComboBox, tbl As String, idxfld As String)
        Dim dstn As New DataSet
        Dim sql As String
        sql = "SELECT * FROM  " & tbl
        da = New MySql.Data.MySqlClient.MySqlDataAdapter(sql, dbconn)
        dstn.Clear()
        da.Fill(dstn, tbl)

        For i = 0 To dstn.Tables(tbl).Rows.Count - 1
            lst.Items.Add(dstn.Tables(tbl).Rows(i).Item(idxfld))
        Next
    End Sub
End Class