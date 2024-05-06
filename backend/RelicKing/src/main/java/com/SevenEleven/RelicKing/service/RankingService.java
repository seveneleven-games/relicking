package com.SevenEleven.RelicKing.service;

import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.repository.RecordRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.aop.AopInvocationException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.*;

@Service
@RequiredArgsConstructor
@Transactional
public class RankingService {

    private final RecordRepository recordRepository;

    public Map<String, Object> getrankings(Member member) {
        Record[] myRecord = new Record[3];
        Arrays.fill(myRecord, Record.builder().member(member).recordId(0).difficulty(0).build());
        List<Record> myRecordList = recordRepository.findByMember(member);
        myRecordList.forEach(record -> myRecord[record.getStage() - 1] = record);
        Map<String, Object> data = new LinkedHashMap<>();
        for (int i = 1; i <= 3; i++) {
            Map<String, Object> stage = new LinkedHashMap<>();

            Map<String, Object> myRank = new LinkedHashMap<>();
            myRank.put("recordId", myRecord[i - 1].getRecordId());
            try {
                myRank.put("rank", recordRepository.findRankByMemberAndStage(member.getMemberId(), i));
            } catch (AopInvocationException e) {
                myRank.put("rank", "-");
            }
            myRank.put("nickname", member.getNickname());
            myRank.put("classNo", member.getCurrentClassNo());
            myRank.put("difficulty", myRecord[i - 1].getDifficulty());
            stage.put("myRank", myRank);

            List<Map<String, Object>> rankList =
                    recordRepository.findTop100ByStageOrderByDifficultyDescEliteKillDescNormalKillDesc(i)
                            .stream().map(record -> {
                                Map<String, Object> rank = new LinkedHashMap<>();
                                rank.put("recordId", record.getRecordId());
                                rank.put("nickname", record.getMember().getNickname());
                                rank.put("classNo", record.getMember().getCurrentClassNo());
                                rank.put("difficulty", record.getDifficulty());
                                return rank;
                            }).toList();
            stage.put("rankList", rankList);
            data.put("stage" + i, stage);
        }

        return data;
    }

}
