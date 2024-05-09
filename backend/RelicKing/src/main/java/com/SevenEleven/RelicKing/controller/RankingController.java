package com.SevenEleven.RelicKing.controller;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.common.security.CustomUserDetails;
import com.SevenEleven.RelicKing.service.RankingService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequiredArgsConstructor
@RequestMapping("/api/rankings")
public class RankingController {

    private final RankingService rankingService;

    @GetMapping()
    public Response getRanking(@AuthenticationPrincipal CustomUserDetails customUserDetails) {

        return new Response(HttpStatus.OK.value(),
            "랭킹 조회 되었습니다.",
            rankingService.getRankings(customUserDetails.getMember()));
    }

    @GetMapping("/{recordId}")
    public Response getDetailedRankingInfo(@PathVariable(value = "recordId") int recordId) {

        return new Response(HttpStatus.OK.value(),
            "랭킹 상세 정보 조회에 성공했습니다.",
            rankingService.getDetailedRanking(recordId));
    }
}
