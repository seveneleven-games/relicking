package com.SevenEleven.RelicKing.repository;

import org.springframework.data.jpa.repository.JpaRepository;

import com.SevenEleven.RelicKing.entity.Member;

public interface MemberRepository extends JpaRepository<Member, Integer> {

}
